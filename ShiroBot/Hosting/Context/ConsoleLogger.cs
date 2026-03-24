using ShiroBot.Core;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting.Context;

internal sealed class ConsoleLogger(string? prefix = null) : IConsoleLogger
{
    private readonly string _prefix = string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix.Trim() + " ";

    public bool IsEnabled
    {
        get => ConsoleHelper.IsEnabled;
        set => ConsoleHelper.IsEnabled = value;
    }

    public void Log(string message) => ConsoleHelper.Log(_prefix + message);
    public void Info(string message) => ConsoleHelper.Info(_prefix + message);
    public void Success(string message) => ConsoleHelper.Success(_prefix + message);
    public void Warning(string message) => ConsoleHelper.Warning(_prefix + message);
    public void Error(string message) => ConsoleHelper.Error(_prefix + message);
}
