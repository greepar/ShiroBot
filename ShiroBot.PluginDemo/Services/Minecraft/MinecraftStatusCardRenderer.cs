using SkiaSharp;

namespace ShiroBot.PluginDemo.Services.Minecraft;

internal static class MinecraftStatusCardRenderer
{
    public static byte[] Render(MinecraftServerStatus status)
    {
        using var surface = SKSurface.Create(new SKImageInfo(1040, 540));
        var canvas = surface.Canvas;
        canvas.Clear(SKColor.Parse("#232323"));

        using var panelPaint = NewPaint("#454545");
        using var dividerPaint = NewPaint("#3a3a3a");
        using var progressTrackPaint = NewPaint("#343434");
        using var progressPaint = NewPaint("#3FE089");
        using var whitePaint = NewTextPaint(SKColors.White);
        using var mutedPaint = NewTextPaint(SKColor.Parse("#A0A0A0"));
        using var greenPaint = NewTextPaint(SKColor.Parse("#5AFF64"));
        using var pinkPaint = NewTextPaint(SKColor.Parse("#F05AF5"));
        using var redPaint = NewTextPaint(SKColor.Parse("#FF6666"));
        using var bluePaint = NewTextPaint(SKColor.Parse("#49A0FF"));

        var cardRect = new SKRoundRect(new SKRect(46, 42, 994, 500), 18, 18);
        canvas.DrawRoundRect(cardRect, panelPaint);
        canvas.DrawLine(194, 42, 194, 500, dividerPaint);
        canvas.DrawLine(194, 128, 994, 128, dividerPaint);
        canvas.DrawLine(194, 390, 994, 390, dividerPaint);

        DrawFavicon(canvas, status.Favicon);
        DrawHeader(canvas, status, greenPaint, redPaint, pinkPaint, whitePaint);
        DrawStats(canvas, status, mutedPaint, whitePaint, bluePaint);
        DrawProgress(canvas, status, whitePaint, progressTrackPaint, progressPaint);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static void DrawHeader(
        SKCanvas canvas,
        MinecraftServerStatus status,
        SKPaint greenPaint,
        SKPaint redPaint,
        SKPaint pinkPaint,
        SKPaint whitePaint)
    {
        using var titleFont = CreateFont(25);
        using var subtitleFont = CreateFont(19);
        using var badgeFont = CreateFont(15, bold: true);

        canvas.DrawText(GetDisplayName(status), 225, 95, titleFont, greenPaint);
        canvas.DrawText($"[{status.VersionName}]", 420, 95, titleFont, redPaint);
        canvas.DrawText(GetDisplayMotd(status.Motd), 225, 126, subtitleFont, pinkPaint);

        using var badgePaint = NewPaint(SKColor.Parse("#F2FFF2"));
        using var badgeDotPaint = NewPaint(SKColor.Parse("#4DD97E"));
        using var badgeTextPaint = NewTextPaint(SKColor.Parse("#4DD97E"));
        var badgeRect = new SKRoundRect(new SKRect(878, 81, 964, 115), 17, 17);
        canvas.DrawRoundRect(badgeRect, badgePaint);
        canvas.DrawCircle(898, 98, 5, badgeDotPaint);
        canvas.DrawText("JAVA", 914, 104, badgeFont, badgeTextPaint);
    }

    private static void DrawStats(
        SKCanvas canvas,
        MinecraftServerStatus status,
        SKPaint mutedPaint,
        SKPaint whitePaint,
        SKPaint bluePaint)
    {
        using var labelFont = CreateFont(16);
        using var valueFont = CreateFont(18, bold: true);
        using var playerFont = CreateFont(16);

        DrawStatBlock(canvas, "地址", $"{status.Host}:{status.Port}", 225, 186, mutedPaint, whitePaint, labelFont, valueFont);
        DrawStatBlock(canvas, "版本", status.VersionName, 421, 186, mutedPaint, bluePaint, labelFont, valueFont);
        DrawStatBlock(canvas, "协议", status.ProtocolVersion.ToString(), 615, 186, mutedPaint, whitePaint, labelFont, valueFont);
        DrawStatBlock(canvas, "延迟", $"{status.LatencyMs}ms", 811, 186, mutedPaint, whitePaint, labelFont, valueFont);

        canvas.DrawText("在线列表", 225, 300, labelFont, mutedPaint);
        var sampleText = status.SamplePlayers.Count == 0
            ? "无"
            : string.Join(", ", status.SamplePlayers.Take(8));
        DrawWrappedText(canvas, sampleText, 225, 334, 720, playerFont, whitePaint, 24);
    }

    private static void DrawProgress(
        SKCanvas canvas,
        MinecraftServerStatus status,
        SKPaint whitePaint,
        SKPaint progressTrackPaint,
        SKPaint progressPaint)
    {
        using var valueFont = CreateFont(18, bold: true);
        var ratio = status.MaxPlayers <= 0 ? 0 : Math.Clamp((float)status.OnlinePlayers / status.MaxPlayers, 0f, 1f);
        canvas.DrawText($"玩家: {status.OnlinePlayers} / {status.MaxPlayers}", 225, 432, valueFont, whitePaint);

        var track = new SKRoundRect(new SKRect(225, 462, 964, 474), 6, 6);
        canvas.DrawRoundRect(track, progressTrackPaint);

        if (ratio > 0)
        {
            var progressWidth = 739 * ratio;
            var progress = new SKRoundRect(new SKRect(225, 462, 225 + progressWidth, 474), 6, 6);
            canvas.DrawRoundRect(progress, progressPaint);
        }
    }

    private static void DrawFavicon(SKCanvas canvas, string? favicon)
    {
        var iconRect = new SKRect(78, 230, 133, 285);
        using var borderPaint = NewPaint(SKColor.Parse("#282828"));
        canvas.DrawRoundRect(new SKRoundRect(new SKRect(74, 226, 137, 289), 12, 12), borderPaint);

        if (TryDecodeFavicon(favicon, out var bitmap))
        {
            using (bitmap)
            {
                canvas.DrawBitmap(bitmap, iconRect);
            }
            return;
        }

        using var fallbackPaint = NewPaint(SKColor.Parse("#151515"));
        canvas.DrawRoundRect(new SKRoundRect(iconRect, 10, 10), fallbackPaint);

        using var letterPaint = NewTextPaint(SKColor.Parse("#F6C04D"));
        using var letterFont = CreateFont(38, bold: true);
        canvas.DrawText("M", 90, 273, letterFont, letterPaint);
    }

    private static bool TryDecodeFavicon(string? favicon, out SKBitmap bitmap)
    {
        bitmap = null!;
        if (string.IsNullOrWhiteSpace(favicon))
        {
            return false;
        }

        const string prefix = "data:image/png;base64,";
        if (!favicon.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            var bytes = Convert.FromBase64String(favicon[prefix.Length..]);
            bitmap = SKBitmap.Decode(bytes);
            return bitmap is not null;
        }
        catch
        {
            return false;
        }
    }

    private static void DrawStatBlock(
        SKCanvas canvas,
        string label,
        string value,
        float x,
        float y,
        SKPaint labelPaint,
        SKPaint valuePaint,
        SKFont labelFont,
        SKFont valueFont)
    {
        canvas.DrawText(label, x, y, labelFont, labelPaint);
        DrawWrappedText(canvas, value, x, y + 34, 150, valueFont, valuePaint, 28);
    }

    private static void DrawWrappedText(
        SKCanvas canvas,
        string text,
        float x,
        float y,
        float maxWidth,
        SKFont font,
        SKPaint paint,
        float lineHeight)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            canvas.DrawText(string.Empty, x, y, font, paint);
            return;
        }

        var currentLine = words[0];
        var currentY = y;

        for (var i = 1; i < words.Length; i++)
        {
            var candidate = currentLine + " " + words[i];
            if (font.MeasureText(candidate, paint) <= maxWidth)
            {
                currentLine = candidate;
                continue;
            }

            canvas.DrawText(currentLine, x, currentY, font, paint);
            currentLine = words[i];
            currentY += lineHeight;
        }

        canvas.DrawText(currentLine, x, currentY, font, paint);
    }

    private static string GetDisplayName(MinecraftServerStatus status)
    {
        var host = status.Host;
        if (string.Equals(host, "mc.hypixel.net", StringComparison.OrdinalIgnoreCase))
        {
            return "Hypixel Network";
        }

        return host;
    }

    private static string GetDisplayMotd(string motd)
    {
        var lines = motd
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        return lines.Length == 0 ? "NO MOTD" : lines[0].ToUpperInvariant();
    }

    private static SKPaint NewPaint(string hex) => NewPaint(SKColor.Parse(hex));

    private static SKPaint NewPaint(SKColor color) =>
        new()
        {
            Color = color,
            IsAntialias = true
        };

    private static SKPaint NewTextPaint(SKColor color) =>
        new()
        {
            Color = color,
            IsAntialias = true
        };

    private static SKFont CreateFont(float size, bool bold = false)
    {
        var style = bold ? SKFontStyle.Bold : SKFontStyle.Normal;
        var familyNames = GetPreferredFontFamilies();

        foreach (var familyName in familyNames)
        {
            var typeface = SKFontManager.Default.MatchFamily(familyName, style);
            if (typeface is not null)
            {
                return new SKFont(typeface, size, 1, 0);
            }
        }

        var fallbackTypeface = SKFontManager.Default.MatchCharacter('中') ?? SKTypeface.Default;
        return new SKFont(fallbackTypeface, size, 1, 0);
    }

    private static string[] GetPreferredFontFamilies()
    {
        if (OperatingSystem.IsWindows())
        {
            return
            [
                "Microsoft YaHei UI",
                "Microsoft YaHei",
                "SimHei",
                "SimSun",
                "Arial Unicode MS"
            ];
        }

        if (OperatingSystem.IsMacOS())
        {
            return
            [
                "PingFang SC",
                "Hiragino Sans GB",
                "Heiti SC",
                "Arial Unicode MS"
            ];
        }

        return
        [
            "Noto Sans CJK SC",
            "Noto Sans SC",
            "WenQuanYi Micro Hei",
            "Source Han Sans SC",
            "Droid Sans Fallback"
        ];
    }
}
