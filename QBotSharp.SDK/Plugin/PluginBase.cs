using QBotSharp.Model.Common;
using QBotSharp.SDK;

namespace QBotSharp.SDK.Plugin;

public abstract class PluginBase : IBotPlugin
{
    private Func<GroupIncomingMessage, Task>? _groupMessageHandler;
    private Func<FriendIncomingMessage, Task>? _friendMessageHandler;
    private Func<MessageRecallEvent, Task>? _messageRecallHandler;
    private Func<FriendRequestEvent, Task>? _friendRequestHandler;
    private Func<GroupJoinRequestEvent, Task>? _groupJoinRequestHandler;
    private Func<GroupInvitedJoinRequestEvent, Task>? _groupInvitedJoinRequestHandler;
    private Func<GroupInvitationEvent, Task>? _groupInvitationHandler;
    private Func<FriendNudgeEvent, Task>? _friendNudgeHandler;
    private Func<FriendFileUploadEvent, Task>? _friendFileUploadHandler;
    private Func<GroupAdminChangeEvent, Task>? _groupAdminChangeHandler;
    private Func<GroupEssenceMessageChangeEvent, Task>? _groupEssenceMessageChangeHandler;
    private Func<GroupMemberIncreaseEvent, Task>? _groupMemberIncreaseHandler;
    private Func<GroupMemberDecreaseEvent, Task>? _groupMemberDecreaseHandler;
    private Func<GroupNameChangeEvent, Task>? _groupNameChangeHandler;
    private Func<GroupMessageReactionEvent, Task>? _groupMessageReactionHandler;
    private Func<GroupMuteEvent, Task>? _groupMuteHandler;
    private Func<GroupWholeMuteEvent, Task>? _groupWholeMuteHandler;
    private Func<GroupNudgeEvent, Task>? _groupNudgeHandler;
    private Func<GroupFileUploadEvent, Task>? _groupFileUploadHandler;

    protected IBotContext Context { get; private set; } = null!;
    protected CommandRouter<GroupIncomingMessage> GroupCommands { get; } = new();
    protected CommandRouter<FriendIncomingMessage> FriendCommands { get; } = new();
    public virtual string Name => GetType().Name;
    public virtual BotComponentMetadata Metadata => new()
    {
        Name = Name,
        Version = "1.0.0"
    };

    public async Task OnLoad(IBotContext context)
    {
        Context = context;
        await OnLoadAsync(context);
        SubscribeEvents(context);
    }

    public async Task OnUnload()
    {
        UnsubscribeEvents();
        await OnUnloadAsync();
    }

    protected virtual Task OnLoadAsync(IBotContext context) => Task.CompletedTask;
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

    private void SubscribeEvents(IBotContext context)
    {
        _groupMessageHandler = OnGroupMessageAsync;
        _friendMessageHandler = OnFriendMessageAsync;
        _messageRecallHandler = OnMessageRecallAsync;
        _friendRequestHandler = OnFriendRequestAsync;
        _groupJoinRequestHandler = OnGroupJoinRequestAsync;
        _groupInvitedJoinRequestHandler = OnGroupInvitedJoinRequestAsync;
        _groupInvitationHandler = OnGroupInvitationAsync;
        _friendNudgeHandler = OnFriendNudgeAsync;
        _friendFileUploadHandler = OnFriendFileUploadAsync;
        _groupAdminChangeHandler = OnGroupAdminChangeAsync;
        _groupEssenceMessageChangeHandler = OnGroupEssenceMessageChangeAsync;
        _groupMemberIncreaseHandler = OnGroupMemberIncreaseAsync;
        _groupMemberDecreaseHandler = OnGroupMemberDecreaseAsync;
        _groupNameChangeHandler = OnGroupNameChangeAsync;
        _groupMessageReactionHandler = OnGroupMessageReactionAsync;
        _groupMuteHandler = OnGroupMuteAsync;
        _groupWholeMuteHandler = OnGroupWholeMuteAsync;
        _groupNudgeHandler = OnGroupNudgeAsync;
        _groupFileUploadHandler = OnGroupFileUploadAsync;

        context.Event.GroupMessageReceived += _groupMessageHandler;
        context.Event.FriendMessageReceived += _friendMessageHandler;
        context.Event.MessageRecall += _messageRecallHandler;
        context.Event.FriendRequest += _friendRequestHandler;
        context.Event.GroupJoinRequest += _groupJoinRequestHandler;
        context.Event.GroupInvitedJoinRequest += _groupInvitedJoinRequestHandler;
        context.Event.GroupInvitation += _groupInvitationHandler;
        context.Event.FriendNudge += _friendNudgeHandler;
        context.Event.FriendFileUpload += _friendFileUploadHandler;
        context.Event.GroupAdminChange += _groupAdminChangeHandler;
        context.Event.GroupEssenceMessageChange += _groupEssenceMessageChangeHandler;
        context.Event.GroupMemberIncrease += _groupMemberIncreaseHandler;
        context.Event.GroupMemberDecrease += _groupMemberDecreaseHandler;
        context.Event.GroupNameChange += _groupNameChangeHandler;
        context.Event.GroupMessageReaction += _groupMessageReactionHandler;
        context.Event.GroupMute += _groupMuteHandler;
        context.Event.GroupWholeMute += _groupWholeMuteHandler;
        context.Event.GroupNudge += _groupNudgeHandler;
        context.Event.GroupFileUpload += _groupFileUploadHandler;
    }

    private void UnsubscribeEvents()
    {
        if (_groupMessageHandler is not null) Context.Event.GroupMessageReceived -= _groupMessageHandler;
        if (_friendMessageHandler is not null) Context.Event.FriendMessageReceived -= _friendMessageHandler;
        if (_messageRecallHandler is not null) Context.Event.MessageRecall -= _messageRecallHandler;
        if (_friendRequestHandler is not null) Context.Event.FriendRequest -= _friendRequestHandler;
        if (_groupJoinRequestHandler is not null) Context.Event.GroupJoinRequest -= _groupJoinRequestHandler;
        if (_groupInvitedJoinRequestHandler is not null) Context.Event.GroupInvitedJoinRequest -= _groupInvitedJoinRequestHandler;
        if (_groupInvitationHandler is not null) Context.Event.GroupInvitation -= _groupInvitationHandler;
        if (_friendNudgeHandler is not null) Context.Event.FriendNudge -= _friendNudgeHandler;
        if (_friendFileUploadHandler is not null) Context.Event.FriendFileUpload -= _friendFileUploadHandler;
        if (_groupAdminChangeHandler is not null) Context.Event.GroupAdminChange -= _groupAdminChangeHandler;
        if (_groupEssenceMessageChangeHandler is not null) Context.Event.GroupEssenceMessageChange -= _groupEssenceMessageChangeHandler;
        if (_groupMemberIncreaseHandler is not null) Context.Event.GroupMemberIncrease -= _groupMemberIncreaseHandler;
        if (_groupMemberDecreaseHandler is not null) Context.Event.GroupMemberDecrease -= _groupMemberDecreaseHandler;
        if (_groupNameChangeHandler is not null) Context.Event.GroupNameChange -= _groupNameChangeHandler;
        if (_groupMessageReactionHandler is not null) Context.Event.GroupMessageReaction -= _groupMessageReactionHandler;
        if (_groupMuteHandler is not null) Context.Event.GroupMute -= _groupMuteHandler;
        if (_groupWholeMuteHandler is not null) Context.Event.GroupWholeMute -= _groupWholeMuteHandler;
        if (_groupNudgeHandler is not null) Context.Event.GroupNudge -= _groupNudgeHandler;
        if (_groupFileUploadHandler is not null) Context.Event.GroupFileUpload -= _groupFileUploadHandler;
    }
}
