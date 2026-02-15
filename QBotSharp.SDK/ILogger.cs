namespace QBotSharp.SDK;

public interface ILogger
{
    static bool IsEnabled;
    static void Log(string message) { }
    static void Info(string message) { }
    static void Success(string message) { }
    static void Warning(string message) { }
    static void Error(string message) { }
}