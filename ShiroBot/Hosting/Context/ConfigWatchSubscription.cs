namespace ShiroBot.Hosting.Context;

internal sealed class ConfigWatchSubscription(
    FileSystemWatcher watcher,
    Timer timer,
    Action unsubscribe) : IDisposable
{
    private int _disposed;

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
        {
            return;
        }

        watcher.EnableRaisingEvents = false;
        unsubscribe();
        watcher.Dispose();
        timer.Dispose();
    }
}
