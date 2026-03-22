namespace QBotSharp.Hosting.Context;

internal sealed class ConfigWatchSubscription(FileSystemWatcher watcher, Timer timer) : IDisposable
{
    private int _disposed;

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        watcher.Dispose();
        timer.Dispose();
    }
}
