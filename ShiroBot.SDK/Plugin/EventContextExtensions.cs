using ShiroBot.Model.Common;

namespace ShiroBot.SDK.Plugin;

public static class EventContextExtensions
{
    public static void OnGroupMessageReceived(this IEventContext context, Action<GroupIncomingMessage> handler) =>
        context.GroupMessageReceived += Wrap(handler);

    public static void OnFriendMessageReceived(this IEventContext context, Action<FriendIncomingMessage> handler) =>
        context.FriendMessageReceived += Wrap(handler);

    public static void OnMessageRecall(this IEventContext context, Action<MessageRecallEvent> handler) =>
        context.MessageRecall += Wrap(handler);

    public static void OnFriendRequest(this IEventContext context, Action<FriendRequestEvent> handler) =>
        context.FriendRequest += Wrap(handler);

    public static void OnGroupJoinRequest(this IEventContext context, Action<GroupJoinRequestEvent> handler) =>
        context.GroupJoinRequest += Wrap(handler);

    public static void OnGroupInvitedJoinRequest(this IEventContext context, Action<GroupInvitedJoinRequestEvent> handler) =>
        context.GroupInvitedJoinRequest += Wrap(handler);

    public static void OnGroupInvitation(this IEventContext context, Action<GroupInvitationEvent> handler) =>
        context.GroupInvitation += Wrap(handler);

    public static void OnFriendNudge(this IEventContext context, Action<FriendNudgeEvent> handler) =>
        context.FriendNudge += Wrap(handler);

    public static void OnFriendFileUpload(this IEventContext context, Action<FriendFileUploadEvent> handler) =>
        context.FriendFileUpload += Wrap(handler);

    public static void OnGroupAdminChange(this IEventContext context, Action<GroupAdminChangeEvent> handler) =>
        context.GroupAdminChange += Wrap(handler);

    public static void OnGroupEssenceMessageChange(this IEventContext context, Action<GroupEssenceMessageChangeEvent> handler) =>
        context.GroupEssenceMessageChange += Wrap(handler);

    public static void OnGroupMemberIncrease(this IEventContext context, Action<GroupMemberIncreaseEvent> handler) =>
        context.GroupMemberIncrease += Wrap(handler);

    public static void OnGroupMemberDecrease(this IEventContext context, Action<GroupMemberDecreaseEvent> handler) =>
        context.GroupMemberDecrease += Wrap(handler);

    public static void OnGroupNameChange(this IEventContext context, Action<GroupNameChangeEvent> handler) =>
        context.GroupNameChange += Wrap(handler);

    public static void OnGroupMessageReaction(this IEventContext context, Action<GroupMessageReactionEvent> handler) =>
        context.GroupMessageReaction += Wrap(handler);

    public static void OnGroupMute(this IEventContext context, Action<GroupMuteEvent> handler) =>
        context.GroupMute += Wrap(handler);

    public static void OnGroupWholeMute(this IEventContext context, Action<GroupWholeMuteEvent> handler) =>
        context.GroupWholeMute += Wrap(handler);

    public static void OnGroupNudge(this IEventContext context, Action<GroupNudgeEvent> handler) =>
        context.GroupNudge += Wrap(handler);

    public static void OnGroupFileUpload(this IEventContext context, Action<GroupFileUploadEvent> handler) =>
        context.GroupFileUpload += Wrap(handler);

    private static Func<TEvent, Task> Wrap<TEvent>(Action<TEvent> handler) =>
        evt =>
        {
            handler(evt);
            return Task.CompletedTask;
        };
}
