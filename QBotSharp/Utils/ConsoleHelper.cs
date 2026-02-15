using Spectre.Console;

namespace QBotSharp.Utils;

public static class ConsoleHelper
{
    private static bool _isEnabled = true;

    /// <summary>
    /// 启用或禁用控制台日志输出
    /// </summary>
    public static bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    /// <summary>
    /// 输出普通日志
    /// </summary>
    public static void Log(string message)
    {
        if (!_isEnabled)
        {
            // AnsiConsole.Clear();
            return;
        }
        AnsiConsole.MarkupLine($"[grey][[{DateTime.Now:HH:mm:ss}]][/] {Markup.Escape(message)}");
    }

    /// <summary>
    /// 输出信息日志 (蓝色)
    /// </summary>
    public static void Info(string message)
    {
        if (!_isEnabled) return;
        AnsiConsole.MarkupLine($"[grey][[{DateTime.Now:HH:mm:ss}]][/] [blue]{Markup.Escape(message)}[/]");
    }

    /// <summary>
    /// 输出成功日志 (绿色)
    /// </summary>
    public static void Success(string message)
    {
        if (!_isEnabled) return;
        AnsiConsole.MarkupLine($"[grey][[{DateTime.Now:HH:mm:ss}]][/] [green]{Markup.Escape(message)}[/]");
    }

    /// <summary>
    /// 输出警告日志 (黄色)
    /// </summary>
    public static void Warning(string message)
    {
        if (!_isEnabled) return;
        AnsiConsole.MarkupLine($"[grey][[{DateTime.Now:HH:mm:ss}]][/] [yellow]{Markup.Escape(message)}[/]");
    }

    /// <summary>
    /// 输出错误日志 (红色)
    /// </summary>
    public static void Error(string message)
    {
        if (!_isEnabled) return;
        AnsiConsole.MarkupLine($"[grey][[{DateTime.Now:HH:mm:ss}]][/] [red]{Markup.Escape(message)}[/]");

    }
}