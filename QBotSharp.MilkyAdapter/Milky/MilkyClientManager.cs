using Milky.Net.Client;
using QBotSharp.SDK.Plugin;
using System.Net.Http.Headers;
using System.Text;

namespace QBotSharp.MilkyAdapter.Milky;

internal static class MilkyClientManager
{
    private static MilkyClient? _instance;
    private static readonly Lock Lock = new();
    private static IConsoleLogger? _logger;

    public static MilkyClient Instance =>
        _instance ?? throw new InvalidOperationException("MilkyClient has not been initialized. Call Initialize() first.");

    public static void Initialize(string baseAddress, string? authToken = null, IConsoleLogger? logger = null)
    {
        lock (Lock)
        {
            _logger = logger;

            if (_instance != null)
            {
                _logger?.Info("MilkyClientManager: 客户端已初始化，跳过重复初始化");
                return;
            }

            var finalBaseAddress = NormalizeBaseAddress(baseAddress);
            if (!string.IsNullOrEmpty(authToken))
            {
                _logger?.Log("MilkyClientManager: 准备使用认证令牌（如果服务器需要 access_token 查询参数，请修改实现）");
            }

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(finalBaseAddress),
                Timeout = TimeSpan.FromSeconds(30)
            };

            if (!string.IsNullOrWhiteSpace(authToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            }

            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Milky.Net.Client/1.0");

            _instance = new MilkyClient(httpClient);
        }
    }

    private static string NormalizeBaseAddress(string baseAddress)
    {
        if (string.IsNullOrWhiteSpace(baseAddress))
        {
            throw new ArgumentException("Milky BaseUrl 不能为空。", nameof(baseAddress));
        }

        return baseAddress.Trim().TrimEnd('/') + "/";
    }
}
