namespace QBotSharp.SDK.Adapter;

/// <summary>
/// 标记主要的 Adapter 实现。
/// 当一个插件中存在多个 IBotAdapter 实现时，标记了此特性的类会被优先加载。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BotAdapterAttribute : Attribute;
