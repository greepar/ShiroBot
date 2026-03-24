using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ShiroBot.PluginDemo.Services.Minecraft;

internal sealed class MinecraftStatusService
{
    public async Task<MinecraftServerStatus> QueryAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        var target = ParseEndpoint(endpoint);
        using var tcpClient = new TcpClient();

        var stopwatch = Stopwatch.StartNew();
        await tcpClient.ConnectAsync(target.Host, target.Port, cancellationToken);
        await using var stream = tcpClient.GetStream();

        await SendHandshakeAsync(stream, target, cancellationToken);
        await SendStatusRequestAsync(stream, cancellationToken);

        var json = await ReadPacketStringAsync(stream, cancellationToken);
        stopwatch.Stop();

        return ParseStatus(target, json, stopwatch.ElapsedMilliseconds);
    }

    private static MinecraftServerTarget ParseEndpoint(string endpoint)
    {
        var trimmed = endpoint.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new InvalidOperationException("服务器地址不能为空");
        }

        var host = trimmed;
        var port = 25565;

        if (trimmed.StartsWith('['))
        {
            var closingIndex = trimmed.IndexOf(']');
            if (closingIndex <= 0)
            {
                throw new InvalidOperationException("IPv6 地址格式无效");
            }

            host = trimmed[1..closingIndex];
            if (closingIndex + 1 < trimmed.Length)
            {
                var portSegment = trimmed[(closingIndex + 2)..];
                if (!int.TryParse(portSegment, out port))
                {
                    throw new InvalidOperationException("端口格式无效");
                }
            }
        }
        else
        {
            var separatorIndex = trimmed.LastIndexOf(':');
            if (separatorIndex > 0 && trimmed.Count(ch => ch == ':') == 1)
            {
                host = trimmed[..separatorIndex];
                if (!int.TryParse(trimmed[(separatorIndex + 1)..], out port))
                {
                    throw new InvalidOperationException("端口格式无效");
                }
            }
        }

        return new MinecraftServerTarget(host, port);
    }

    private static async Task SendHandshakeAsync(NetworkStream stream, MinecraftServerTarget target, CancellationToken cancellationToken)
    {
        using var payload = new MemoryStream();
        WriteVarInt(payload, 0);
        WriteVarInt(payload, 758);
        WriteString(payload, target.Host);
        WriteUnsignedShort(payload, (ushort)target.Port);
        WriteVarInt(payload, 1);
        await WritePacketAsync(stream, payload.ToArray(), cancellationToken);
    }

    private static async Task SendStatusRequestAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        await WritePacketAsync(stream, [0], cancellationToken);
    }

    private static async Task<string> ReadPacketStringAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        _ = await ReadVarIntAsync(stream, cancellationToken);
        var packetId = await ReadVarIntAsync(stream, cancellationToken);
        if (packetId != 0)
        {
            throw new InvalidOperationException($"收到未知的状态响应包: {packetId}");
        }

        var stringLength = await ReadVarIntAsync(stream, cancellationToken);
        var buffer = ArrayPool<byte>.Shared.Rent(stringLength);
        try
        {
            await ReadExactAsync(stream, buffer.AsMemory(0, stringLength), cancellationToken);
            return Encoding.UTF8.GetString(buffer, 0, stringLength);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private static async Task WritePacketAsync(NetworkStream stream, byte[] payload, CancellationToken cancellationToken)
    {
        using var packet = new MemoryStream();
        WriteVarInt(packet, payload.Length);
        packet.Write(payload, 0, payload.Length);
        packet.Position = 0;
        await packet.CopyToAsync(stream, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    private static async Task<int> ReadVarIntAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        var value = 0;
        var position = 0;

        while (true)
        {
            var current = await ReadByteAsync(stream, cancellationToken);
            value |= (current & 0x7F) << position;
            if ((current & 0x80) == 0)
            {
                return value;
            }

            position += 7;
            if (position >= 35)
            {
                throw new InvalidOperationException("VarInt 过长");
            }
        }
    }

    private static async Task<byte> ReadByteAsync(NetworkStream stream, CancellationToken cancellationToken)
    {
        var buffer = new byte[1];
        await ReadExactAsync(stream, buffer, cancellationToken);
        return buffer[0];
    }

    private static async Task ReadExactAsync(NetworkStream stream, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var totalRead = 0;
        while (totalRead < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer[totalRead..], cancellationToken);
            if (read == 0)
            {
                throw new InvalidOperationException("服务器过早断开连接");
            }

            totalRead += read;
        }
    }

    private static void WriteVarInt(Stream stream, int value)
    {
        uint unsigned = (uint)value;
        while (true)
        {
            if ((unsigned & ~0x7Fu) == 0)
            {
                stream.WriteByte((byte)unsigned);
                return;
            }

            stream.WriteByte((byte)((unsigned & 0x7F) | 0x80));
            unsigned >>= 7;
        }
    }

    private static void WriteUnsignedShort(Stream stream, ushort value)
    {
        stream.WriteByte((byte)(value >> 8));
        stream.WriteByte((byte)(value & 0xFF));
    }

    private static void WriteString(Stream stream, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        WriteVarInt(stream, bytes.Length);
        stream.Write(bytes, 0, bytes.Length);
    }

    private static MinecraftServerStatus ParseStatus(MinecraftServerTarget target, string json, long latencyMs)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        var version = root.TryGetProperty("version", out var versionElement) ? versionElement : default;
        var players = root.TryGetProperty("players", out var playersElement) ? playersElement : default;

        string? favicon = root.TryGetProperty("favicon", out var faviconElement) ? faviconElement.GetString() : null;
        var motd = FlattenDescription(root.TryGetProperty("description", out var description) ? description : default);
        var samples = ParsePlayerSamples(players);

        return new MinecraftServerStatus(
            target.Host,
            target.Port,
            version.ValueKind == JsonValueKind.Object && version.TryGetProperty("name", out var versionName) ? versionName.GetString() ?? "Unknown" : "Unknown",
            version.ValueKind == JsonValueKind.Object && version.TryGetProperty("protocol", out var protocol) ? protocol.GetInt32() : 0,
            players.ValueKind == JsonValueKind.Object && players.TryGetProperty("online", out var online) ? online.GetInt32() : 0,
            players.ValueKind == JsonValueKind.Object && players.TryGetProperty("max", out var max) ? max.GetInt32() : 0,
            samples,
            motd,
            favicon,
            latencyMs);
    }

    private static IReadOnlyList<string> ParsePlayerSamples(JsonElement players)
    {
        if (players.ValueKind != JsonValueKind.Object ||
            !players.TryGetProperty("sample", out var sample) ||
            sample.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var items = new List<string>();
        foreach (var entry in sample.EnumerateArray())
        {
            if (entry.TryGetProperty("name", out var name))
            {
                var text = name.GetString();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    items.Add(text);
                }
            }
        }

        return items;
    }

    private static string FlattenDescription(JsonElement description)
    {
        if (description.ValueKind == JsonValueKind.Undefined || description.ValueKind == JsonValueKind.Null)
        {
            return "No MOTD";
        }

        var builder = new StringBuilder();
        AppendDescription(builder, description);
        var result = StripMinecraftFormatting(builder.ToString()).Trim();
        return string.IsNullOrWhiteSpace(result) ? "No MOTD" : result;
    }

    private static void AppendDescription(StringBuilder builder, JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                builder.Append(element.GetString());
                break;
            case JsonValueKind.Object:
                if (element.TryGetProperty("text", out var text) && text.ValueKind == JsonValueKind.String)
                {
                    builder.Append(text.GetString());
                }

                if (element.TryGetProperty("extra", out var extra) && extra.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in extra.EnumerateArray())
                    {
                        AppendDescription(builder, item);
                    }
                }

                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    AppendDescription(builder, item);
                }

                break;
        }
    }

    private static string StripMinecraftFormatting(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var builder = new StringBuilder(value.Length);
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] == '§' && i + 1 < value.Length)
            {
                i++;
                continue;
            }

            builder.Append(value[i]);
        }

        return builder.ToString();
    }
}

internal sealed record MinecraftServerTarget(string Host, int Port);

internal sealed record MinecraftServerStatus(
    string Host,
    int Port,
    string VersionName,
    int ProtocolVersion,
    int OnlinePlayers,
    int MaxPlayers,
    IReadOnlyList<string> SamplePlayers,
    string Motd,
    string? Favicon,
    long LatencyMs);
