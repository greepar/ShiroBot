using ShiroBot.Model.Common;
using ShiroBot.SDK.Core;
using System.Reflection;

namespace ShiroBot.SDK.Plugin;

public sealed record MessageRouteDescriptor(MessageRouteMatchType MatchType, string? Pattern);
public enum MessageRouteMatchType
{
    Exact,
    Prefix,
    All
}

public abstract class PluginBase : IBotPlugin, IBotEventSubscriber
{
    private static readonly MethodInfo CreateEventDispatcherMethod =
        typeof(PluginBase).GetMethod(nameof(CreateEventDispatcher), BindingFlags.Static | BindingFlags.NonPublic)
        ?? throw new InvalidOperationException($"Failed to locate {nameof(CreateEventDispatcher)}.");

    private static readonly IReadOnlyDictionary<Type, Func<PluginBase, Event, Task>> EventDispatchers =
        CreateEventDispatchers();

    protected IBotContext Context { get; private set; } = null!;
    protected CommandRouter<GroupIncomingMessage> GroupCommands { get; } = new();
    protected CommandRouter<FriendIncomingMessage> FriendCommands { get; } = new();
    private static BotEventSubscriptions Subscriptions => BotEventSubscriptions.None;
    public virtual string Name => GetType().Name;
    public virtual BotComponentMetadata Metadata => new()
    {
        Name = Name,
        Version = "1.0.0"
    };

    public async Task OnLoad(IBotContext context)
    {
        Context = context;
        await LoadAsync();
    }

    public async Task OnUnload()
    {
        await OnUnloadAsync();
        GroupCommands.Clear();
        FriendCommands.Clear();
        Context = null!;
    }

    protected virtual Task LoadAsync() => Task.CompletedTask;
    protected virtual Task OnUnloadAsync() => Task.CompletedTask;

    protected virtual async Task OnGroupMessageAsync(GroupIncomingMessage message)
    {
        if (await BeforeDispatchGroupCommandAsync(message))
        {
            await GroupCommands.DispatchAsync(message.GetPlainText().Trim(), message);
        }
    }
    protected virtual async Task OnFriendMessageAsync(FriendIncomingMessage message)
    {
        if (await BeforeDispatchFriendCommandAsync(message))
        {
            await FriendCommands.DispatchAsync(message.GetPlainText().Trim(), message);
        }
    }

    protected virtual Task<bool> BeforeDispatchGroupCommandAsync(GroupIncomingMessage message) =>
        Task.FromResult(true);

    protected virtual Task<bool> BeforeDispatchFriendCommandAsync(FriendIncomingMessage message) =>
        Task.FromResult(true);

    protected virtual Task OnMessageRecallAsync(MessageRecallEvent e) => Task.CompletedTask;
    protected virtual Task OnFriendRequestAsync(FriendRequestEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupJoinRequestAsync(GroupJoinRequestEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupInvitedJoinRequestAsync(GroupInvitedJoinRequestEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupInvitationAsync(GroupInvitationEvent e) => Task.CompletedTask;
    protected virtual Task OnFriendNudgeAsync(FriendNudgeEvent e) => Task.CompletedTask;
    protected virtual Task OnFriendFileUploadAsync(FriendFileUploadEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupAdminChangeAsync(GroupAdminChangeEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupEssenceMessageChangeAsync(GroupEssenceMessageChangeEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupMemberIncreaseAsync(GroupMemberIncreaseEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupMemberDecreaseAsync(GroupMemberDecreaseEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupNameChangeAsync(GroupNameChangeEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupMessageReactionAsync(GroupMessageReactionEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupMuteAsync(GroupMuteEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupWholeMuteAsync(GroupWholeMuteEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupNudgeAsync(GroupNudgeEvent e) => Task.CompletedTask;
    protected virtual Task OnGroupFileUploadAsync(GroupFileUploadEvent e) => Task.CompletedTask;

    Task IBotEventSubscriber.OnEventAsync(Event e) =>
        EventDispatchers.TryGetValue(e.GetType(), out var dispatcher)
            ? dispatcher(this, e)
            : Task.CompletedTask;
    public IReadOnlyList<MessageRouteDescriptor> GetGroupMessageRoutes() => GroupCommands.Routes;
    public IReadOnlyList<MessageRouteDescriptor> GetFriendMessageRoutes() => FriendCommands.Routes;
    public bool RequiresGroupMessageBroadcast() => Overrides<GroupIncomingMessage>(GetType(), nameof(OnGroupMessageAsync));
    public bool RequiresFriendMessageBroadcast() => Overrides<FriendIncomingMessage>(GetType(), nameof(OnFriendMessageAsync));

    public BotEventSubscriptions GetEffectiveSubscriptions()
    {
        var subscriptions = Subscriptions;

        if (FriendCommands.HasRoutes)
        {
            subscriptions |= BotEventSubscriptions.FriendMessage;
        }

        if (GroupCommands.HasRoutes)
        {
            subscriptions |= BotEventSubscriptions.GroupMessage;
        }

        subscriptions |= InferOverriddenEventSubscriptions();

        return subscriptions;
    }

    private BotEventSubscriptions InferOverriddenEventSubscriptions()
    {
        var subscriptions = BotEventSubscriptions.None;
        var runtimeType = GetType();

        if (Overrides<GroupIncomingMessage>(runtimeType, nameof(OnGroupMessageAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMessage;
        }

        if (Overrides<FriendIncomingMessage>(runtimeType, nameof(OnFriendMessageAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendMessage;
        }

        if (Overrides<MessageRecallEvent>(runtimeType, nameof(OnMessageRecallAsync)))
        {
            subscriptions |= BotEventSubscriptions.MessageRecall;
        }

        if (Overrides<FriendRequestEvent>(runtimeType, nameof(OnFriendRequestAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendRequest;
        }

        if (Overrides<GroupJoinRequestEvent>(runtimeType, nameof(OnGroupJoinRequestAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupJoinRequest;
        }

        if (Overrides<GroupInvitedJoinRequestEvent>(runtimeType, nameof(OnGroupInvitedJoinRequestAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupInvitedJoinRequest;
        }

        if (Overrides<GroupInvitationEvent>(runtimeType, nameof(OnGroupInvitationAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupInvitation;
        }

        if (Overrides<FriendNudgeEvent>(runtimeType, nameof(OnFriendNudgeAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendNudge;
        }

        if (Overrides<FriendFileUploadEvent>(runtimeType, nameof(OnFriendFileUploadAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendFileUpload;
        }

        if (Overrides<GroupAdminChangeEvent>(runtimeType, nameof(OnGroupAdminChangeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupAdminChange;
        }

        if (Overrides<GroupEssenceMessageChangeEvent>(runtimeType, nameof(OnGroupEssenceMessageChangeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupEssenceMessageChange;
        }

        if (Overrides<GroupMemberIncreaseEvent>(runtimeType, nameof(OnGroupMemberIncreaseAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMemberIncrease;
        }

        if (Overrides<GroupMemberDecreaseEvent>(runtimeType, nameof(OnGroupMemberDecreaseAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMemberDecrease;
        }

        if (Overrides<GroupNameChangeEvent>(runtimeType, nameof(OnGroupNameChangeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupNameChange;
        }

        if (Overrides<GroupMessageReactionEvent>(runtimeType, nameof(OnGroupMessageReactionAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMessageReaction;
        }

        if (Overrides<GroupMuteEvent>(runtimeType, nameof(OnGroupMuteAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMute;
        }

        if (Overrides<GroupWholeMuteEvent>(runtimeType, nameof(OnGroupWholeMuteAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupWholeMute;
        }

        if (Overrides<GroupNudgeEvent>(runtimeType, nameof(OnGroupNudgeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupNudge;
        }

        if (Overrides<GroupFileUploadEvent>(runtimeType, nameof(OnGroupFileUploadAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupFileUpload;
        }

        return subscriptions;
    }

    private static bool Overrides<TEvent>(Type runtimeType, string methodName)
    {
        var method = runtimeType.GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(TEvent)],
            modifiers: null);

        return method is not null && method.DeclaringType != method.GetBaseDefinition().DeclaringType;
    }

    private static IReadOnlyDictionary<Type, Func<PluginBase, Event, Task>> CreateEventDispatchers()
    {
        var dispatchers = new Dictionary<Type, Func<PluginBase, Event, Task>>();

        foreach (var method in typeof(PluginBase).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
        {
            if (!method.Name.StartsWith("On", StringComparison.Ordinal) ||
                !method.Name.EndsWith("Async", StringComparison.Ordinal) ||
                method.ReturnType != typeof(Task))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 1 || !typeof(Event).IsAssignableFrom(parameters[0].ParameterType))
            {
                continue;
            }

            var eventType = parameters[0].ParameterType;
            dispatchers[eventType] = (Func<PluginBase, Event, Task>)CreateEventDispatcherMethod
                .MakeGenericMethod(eventType)
                .Invoke(null, [method])!;
        }

        return dispatchers;
    }

    private static Func<PluginBase, Event, Task> CreateEventDispatcher<TEvent>(MethodInfo method)
        where TEvent : Event
    {
        var handler = (Func<PluginBase, TEvent, Task>)Delegate.CreateDelegate(
            typeof(Func<PluginBase, TEvent, Task>),
            method);

        return (plugin, e) => handler(plugin, (TEvent)e);
    }
}
