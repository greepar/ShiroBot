using QBotSharp.SDK.Plugin;

namespace QBotSharp.SDK.Abstractions;

public static class BotLog
{
    private static readonly AsyncLocal<IConsoleLogger?> CurrentLogger = new();
    private static IConsoleLogger _defaultLogger = new NullConsoleLogger();

    public static IConsoleLogger Logger => CurrentLogger.Value ?? _defaultLogger;

    public static void SetDefault(IConsoleLogger logger)
    {
        _defaultLogger = logger ?? new NullConsoleLogger();
    }

    public static IDisposable BeginScope(IConsoleLogger? logger)
    {
        var previous = CurrentLogger.Value;
        CurrentLogger.Value = logger;
        return new Scope(() => CurrentLogger.Value = previous);
    }

    public static void RunScoped(IConsoleLogger? logger, Action action)
    {
        using var _ = BeginScope(logger);
        action();
    }

    public static T RunScoped<T>(IConsoleLogger? logger, Func<T> func)
    {
        using var _ = BeginScope(logger);
        return func();
    }

    public static async Task RunScoped(IConsoleLogger? logger, Func<Task> action)
    {
        using var _ = BeginScope(logger);
        await action();
    }

    public static async Task<T> RunScoped<T>(IConsoleLogger? logger, Func<Task<T>> func)
    {
        using var _ = BeginScope(logger);
        return await func();
    }

    public static void Log(string message) => Logger.Log(message);
    public static void Info(string message) => Logger.Info(message);
    public static void Success(string message) => Logger.Success(message);
    public static void Warning(string message) => Logger.Warning(message);
    public static void Error(string message) => Logger.Error(message);

    private sealed class Scope(Action onDispose) : IDisposable
    {
        public void Dispose() => onDispose();
    }

    private sealed class NullConsoleLogger : IConsoleLogger
    {
        public bool IsEnabled { get; set; } = true;

        public void Log(string message) { }
        public void Info(string message) { }
        public void Success(string message) { }
        public void Warning(string message) { }
        public void Error(string message) { }
    }
}
