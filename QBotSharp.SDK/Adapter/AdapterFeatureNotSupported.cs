namespace QBotSharp.SDK.Adapter;

internal static class AdapterFeatureNotSupported
{
    public static Task NotSupportedAsync(string feature)
        => throw new NotSupportedException($"Current adapter does not support '{feature}'.");

    public static Task<T> NotSupportedAsync<T>(string feature)
        => throw new NotSupportedException($"Current adapter does not support '{feature}'.");
}
