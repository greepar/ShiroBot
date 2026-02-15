using Milky.Net.Client;

namespace QBotSharp.MilkyAdapter.Milky;

internal static class MilkyClientManager
{
    private static MilkyClient? _instance;
    private static readonly Lock Lock = new();

    public static MilkyClient Instance => _instance ?? throw new InvalidOperationException("MilkyClient has not been initialized. Call Initialize() first.");

    public static void Initialize(string baseAddress, string? authToken = null)
    {
        lock (Lock)
        {
            if (_instance != null)
            {
                Console.WriteLine("MilkyClientManager: 客户端已初始化，跳过重复初始化");
                return;
            }

            Console.WriteLine($"MilkyClientManager: 正在初始化客户端，服务器地址: {baseAddress}");
            
            var finalBaseAddress = baseAddress;
            if (!string.IsNullOrEmpty(authToken))
            {
                // 确保 baseAddress 以 / 结尾
                if (!baseAddress.EndsWith('/'))
                {
                    finalBaseAddress = baseAddress + "/";
                }
                Console.WriteLine($"MilkyClientManager: 准备使用认证令牌（如果服务器需要 access_token 查询参数，请修改实现）");
            }
            
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(finalBaseAddress),
                DefaultRequestHeaders =
                {
                    { "Authorization", $"Bearer {authToken}" },
                },
                Timeout = TimeSpan.FromSeconds(30) // 设置默认超时
            };
            
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Milky.Net.Client/1.0");
            
            _instance = new MilkyClient(httpClient);
        }
    }
}