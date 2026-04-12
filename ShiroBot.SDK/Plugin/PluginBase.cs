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

    Task IBotEventSubscriber.OnGroupMessageAsync(GroupIncomingMessage message) => OnGroupMessageAsync(message);
    Task IBotEventSubscriber.OnFriendMessageAsync(FriendIncomingMessage message) => OnFriendMessageAsync(message);
    Task IBotEventSubscriber.OnMessageRecallAsync(MessageRecallEvent e) => OnMessageRecallAsync(e);
    Task IBotEventSubscriber.OnFriendRequestAsync(FriendRequestEvent e) => OnFriendRequestAsync(e);
    Task IBotEventSubscriber.OnGroupJoinRequestAsync(GroupJoinRequestEvent e) => OnGroupJoinRequestAsync(e);
    Task IBotEventSubscriber.OnGroupInvitedJoinRequestAsync(GroupInvitedJoinRequestEvent e) => OnGroupInvitedJoinRequestAsync(e);
    Task IBotEventSubscriber.OnGroupInvitationAsync(GroupInvitationEvent e) => OnGroupInvitationAsync(e);
    Task IBotEventSubscriber.OnFriendNudgeAsync(FriendNudgeEvent e) => OnFriendNudgeAsync(e);
    Task IBotEventSubscriber.OnFriendFileUploadAsync(FriendFileUploadEvent e) => OnFriendFileUploadAsync(e);
    Task IBotEventSubscriber.OnGroupAdminChangeAsync(GroupAdminChangeEvent e) => OnGroupAdminChangeAsync(e);
    Task IBotEventSubscriber.OnGroupEssenceMessageChangeAsync(GroupEssenceMessageChangeEvent e) => OnGroupEssenceMessageChangeAsync(e);
    Task IBotEventSubscriber.OnGroupMemberIncreaseAsync(GroupMemberIncreaseEvent e) => OnGroupMemberIncreaseAsync(e);
    Task IBotEventSubscriber.OnGroupMemberDecreaseAsync(GroupMemberDecreaseEvent e) => OnGroupMemberDecreaseAsync(e);
    Task IBotEventSubscriber.OnGroupNameChangeAsync(GroupNameChangeEvent e) => OnGroupNameChangeAsync(e);
    Task IBotEventSubscriber.OnGroupMessageReactionAsync(GroupMessageReactionEvent e) => OnGroupMessageReactionAsync(e);
    Task IBotEventSubscriber.OnGroupMuteAsync(GroupMuteEvent e) => OnGroupMuteAsync(e);
    Task IBotEventSubscriber.OnGroupWholeMuteAsync(GroupWholeMuteEvent e) => OnGroupWholeMuteAsync(e);
    Task IBotEventSubscriber.OnGroupNudgeAsync(GroupNudgeEvent e) => OnGroupNudgeAsync(e);
    Task IBotEventSubscriber.OnGroupFileUploadAsync(GroupFileUploadEvent e) => OnGroupFileUploadAsync(e);
    public IReadOnlyList<MessageRouteDescriptor> GetGroupMessageRoutes() => GroupCommands.Routes;
    public IReadOnlyList<MessageRouteDescriptor> GetFriendMessageRoutes() => FriendCommands.Routes;
    public bool RequiresGroupMessageBroadcast() => Overrides(GetType(), nameof(OnGroupMessageAsync));
    public bool RequiresFriendMessageBroadcast() => Overrides(GetType(), nameof(OnFriendMessageAsync));

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

        if (Overrides(runtimeType, nameof(OnGroupMessageAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMessage;
        }

        if (Overrides(runtimeType, nameof(OnFriendMessageAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendMessage;
        }

        if (Overrides(runtimeType, nameof(OnMessageRecallAsync)))
        {
            subscriptions |= BotEventSubscriptions.MessageRecall;
        }

        if (Overrides(runtimeType, nameof(OnFriendRequestAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendRequest;
        }

        if (Overrides(runtimeType, nameof(OnGroupJoinRequestAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupJoinRequest;
        }

        if (Overrides(runtimeType, nameof(OnGroupInvitedJoinRequestAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupInvitedJoinRequest;
        }

        if (Overrides(runtimeType, nameof(OnGroupInvitationAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupInvitation;
        }

        if (Overrides(runtimeType, nameof(OnFriendNudgeAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendNudge;
        }

        if (Overrides(runtimeType, nameof(OnFriendFileUploadAsync)))
        {
            subscriptions |= BotEventSubscriptions.FriendFileUpload;
        }

        if (Overrides(runtimeType, nameof(OnGroupAdminChangeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupAdminChange;
        }

        if (Overrides(runtimeType, nameof(OnGroupEssenceMessageChangeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupEssenceMessageChange;
        }

        if (Overrides(runtimeType, nameof(OnGroupMemberIncreaseAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMemberIncrease;
        }

        if (Overrides(runtimeType, nameof(OnGroupMemberDecreaseAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMemberDecrease;
        }

        if (Overrides(runtimeType, nameof(OnGroupNameChangeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupNameChange;
        }

        if (Overrides(runtimeType, nameof(OnGroupMessageReactionAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMessageReaction;
        }

        if (Overrides(runtimeType, nameof(OnGroupMuteAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupMute;
        }

        if (Overrides(runtimeType, nameof(OnGroupWholeMuteAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupWholeMute;
        }

        if (Overrides(runtimeType, nameof(OnGroupNudgeAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupNudge;
        }

        if (Overrides(runtimeType, nameof(OnGroupFileUploadAsync)))
        {
            subscriptions |= BotEventSubscriptions.GroupFileUpload;
        }

        return subscriptions;
    }

    private static bool Overrides(Type runtimeType, string methodName)
    {
        var method = runtimeType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        return method is not null && method.GetBaseDefinition().DeclaringType != typeof(PluginBase);
    }
}
