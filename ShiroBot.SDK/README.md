# QBotSharp.SDK

`QBotSharp.SDK` 提供插件和 adapter 的宿主契约，以及一层更适合开发者直接使用的高层 API。

## 插件开发

推荐继承 [PluginBase.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.SDK/Plugin/PluginBase.cs)，不要直接手动订阅 `context.Event.*`。

```csharp
using QBotSharp.Model.Common;
using QBotSharp.SDK;
using QBotSharp.SDK.Plugin;

public sealed class HelloPlugin : PluginBase
{
    public override string Name => "HelloPlugin";

    public BotComponentMetadata Metadata { get; } = new()
    {
        Name = "HelloPlugin",
        Version = "1.0.0",
        ApiVersion = "1.0.0",
        Description = "示例插件"
    };

    protected override async Task OnGroupMessageAsync(GroupIncomingMessage message)
    {
        var text = message.GetPlainText();
        if (!text.StartsWith("#hello", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        await Context.Message.ReplyTextAsync(message, "hello");
    }
}
```

如果插件主要是处理多个消息前缀，直接使用 `PluginBase` 自带的 `FriendCommands` 和 `GroupCommands`。

```csharp
public sealed class MyPlugin : PluginBase
{
    protected override Task OnLoadAsync(IBotContext context)
    {
        FriendCommands.Map("#help", HandleHelpAsync);
        GroupCommands.Map("#ping", HandlePingAsync);
        GroupCommands.Map("#status", HandleStatusAsync);
        return Task.CompletedTask;
    }

    private Task HandlePingAsync(GroupIncomingMessage message) =>
        Context.Message.ReplyTextAsync(message, "pong");
}
```

### PluginBase 可直接重写的事件

- `OnFriendMessageAsync`
- `OnGroupMessageAsync`
- `OnMessageRecallAsync`
- `OnFriendRequestAsync`
- `OnGroupJoinRequestAsync`
- `OnGroupInvitedJoinRequestAsync`
- `OnGroupInvitationAsync`
- `OnFriendNudgeAsync`
- `OnFriendFileUploadAsync`
- `OnGroupAdminChangeAsync`
- `OnGroupEssenceMessageChangeAsync`
- `OnGroupMemberIncreaseAsync`
- `OnGroupMemberDecreaseAsync`
- `OnGroupNameChangeAsync`
- `OnGroupMessageReactionAsync`
- `OnGroupMuteAsync`
- `OnGroupWholeMuteAsync`
- `OnGroupNudgeAsync`
- `OnGroupFileUploadAsync`

如果你没有继承 `PluginBase`，但又想手动订阅事件，现在也可以直接用 `Action<T>` 风格，不必每次手写 `Task.CompletedTask`：

```csharp
context.Event.OnFriendFileUpload(e =>
{
    context.Logger.Info($"收到文件: {e.FileName}");
});
```

这些同步注册辅助在 [EventContextExtensions.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.SDK/Plugin/EventContextExtensions.cs)。

### 常用消息快捷方法

这些扩展方法在 [MessageContextExtensions.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.SDK/Plugin/MessageContextExtensions.cs)：

```csharp
await Context.Message.SendPrivateTextAsync(userId, "hello");
await Context.Message.SendGroupTextAsync(groupId, "hello");
await Context.Message.ReplyTextAsync(friendMessage, "pong");
await Context.Message.ReplyTextAsync(groupMessage, "pong");
```

### 命令路由

命令路由器在 [CommandRouter.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.SDK/Plugin/CommandRouter.cs)。

适合场景：

- 一个插件监听多个关键词前缀
- 私聊命令和群命令分开管理
- 不想在 `OnGroupMessageAsync` 里写很长的 `if/else`

纯文本提取在 [IncomingMessageExtensions.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.SDK/Plugin/IncomingMessageExtensions.cs)：

```csharp
var text = message.GetPlainText();
```

## 插件配置

插件配置通过 `context.Config` 访问，配置文件路径固定在：

`plugins/<PluginFolder>/config.toml`

常用操作：

```csharp
var config = context.Config.Load<MyPluginConfig>();
context.Config.Save(config);
```

首次读取时如果文件不存在，宿主会自动生成默认配置文件。

### 插件配置热重载

如果插件需要监听配置变化，可以主动订阅：

```csharp
private IDisposable? _configWatcher;

protected override Task OnLoadAsync(IBotContext context)
{
    _configWatcher = context.Config.Watch<MyPluginConfig>(updated =>
    {
        // 应用新配置
    });

    return Task.CompletedTask;
}

protected override Task OnUnloadAsync()
{
    _configWatcher?.Dispose();
    return Task.CompletedTask;
}
```

说明：

- 不调用 `Watch<T>()` 就不会监听
- 默认防抖时间是 `500ms`
- 返回值必须在插件卸载时释放

## 插件群路由

宿主支持按群号限制某些插件接收群相关事件。配置写在主配置 `config.toml`：

```toml
[plugin_routes.default]
mode = "blacklist"
groups = []

[plugin_routes.plugins.DemoPlugin]
mode = "whitelist"
groups = [622603336, 742274811]
```

规则说明：

- `whitelist`: 只有列表中的群会分发给该插件
- `blacklist`: 除列表中的群外，其它群都会分发给该插件
- `plugin_routes.default`: 给所有没有单独配置的插件使用
- `plugin_routes.plugins.<PluginName>`: 覆盖单个插件

当前会被宿主统一过滤的群事件包括：

- 群消息
- 群撤回
- 入群请求 / 邀请
- 群管理员变更
- 群成员增减
- 群名称变更
- 群消息表情回应
- 群禁言 / 全员禁言
- 群戳一戳
- 群文件上传

## 插件日志

`Logger` 现在直接在 `IBotContext` 上可用，不需要再做类型判断：

```csharp
protected override Task OnLoadAsync(IBotContext context)
{
    context.Logger.Info($"插件 {Name} 已加载");
    return Task.CompletedTask;
}
```

可用方法：

- `Log(...)`
- `Info(...)`
- `Success(...)`
- `Warning(...)`
- `Error(...)`

如果插件继承的是 [PluginBase.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.SDK/Plugin/PluginBase.cs)，也可以直接用：

```csharp
BotLogger.Info($"插件 {Name} 已加载");
```

## Adapter 开发

adapter 需要实现 [IBotAdapter.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.SDK/IBotAdapter.cs)。

```csharp
public sealed class MyAdapter : IBotAdapter
{
    public string Name => "my-adapter";
    public BotComponentMetadata Metadata { get; } = new()
    {
        Name = "MyAdapter",
        Version = "1.0.0",
        ApiVersion = "1.0.0"
    };

    public IConfigContext Config { get; set; } = null!;

    public async Task StartAsync()
    {
        var config = Config.Load<MyAdapterConfig>();
        Config.Save(config);
    }
}
```

adapter 配置文件路径固定在：

`adapters/<AdapterFolder>/config.toml`

### Adapter 配置热重载

如果 adapter 需要监听配置变化：

```csharp
private IDisposable? _configWatcher;

public async Task StartAsync()
{
    _configWatcher = Config.Watch<MyAdapterConfig>(updated =>
    {
        // 应用新配置
    });
}

public Task StopAsync()
{
    _configWatcher?.Dispose();
    return Task.CompletedTask;
}
```

### Adapter 元数据和停止接口

`Metadata`、`Config` 和 `StopAsync()` 已经并入 `IBotAdapter`。

宿主会直接读取 `adapter.Metadata`，初始化 `adapter.Config`，并在退出时调用 `adapter.StopAsync()` 做资源释放。

## 示例

当前可以直接参考：

- [DemoPlugin.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.PluginDemo/DemoPlugin.cs)
- [DemoPluginConfig.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.PluginDemo/DemoPluginConfig.cs)
- [DemoAdapter.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.AdapterDemo/DemoAdapter.cs)
- [MilkyAdapter.cs](/C:/Users/greep/RiderProjects/QB/QBotSharp/QBotSharp.MilkyAdapter/MilkyAdapter.cs)
