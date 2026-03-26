using ShiroBot.Model.Common;

namespace ShiroBot.SDK.Plugin;

public interface IBotEventSubscriber
{
    Task OnGroupMessageAsync(GroupIncomingMessage message);
    Task OnFriendMessageAsync(FriendIncomingMessage message);
    Task OnMessageRecallAsync(MessageRecallEvent e);
    Task OnFriendRequestAsync(FriendRequestEvent e);
    Task OnGroupJoinRequestAsync(GroupJoinRequestEvent e);
    Task OnGroupInvitedJoinRequestAsync(GroupInvitedJoinRequestEvent e);
    Task OnGroupInvitationAsync(GroupInvitationEvent e);
    Task OnFriendNudgeAsync(FriendNudgeEvent e);
    Task OnFriendFileUploadAsync(FriendFileUploadEvent e);
    Task OnGroupAdminChangeAsync(GroupAdminChangeEvent e);
    Task OnGroupEssenceMessageChangeAsync(GroupEssenceMessageChangeEvent e);
    Task OnGroupMemberIncreaseAsync(GroupMemberIncreaseEvent e);
    Task OnGroupMemberDecreaseAsync(GroupMemberDecreaseEvent e);
    Task OnGroupNameChangeAsync(GroupNameChangeEvent e);
    Task OnGroupMessageReactionAsync(GroupMessageReactionEvent e);
    Task OnGroupMuteAsync(GroupMuteEvent e);
    Task OnGroupWholeMuteAsync(GroupWholeMuteEvent e);
    Task OnGroupNudgeAsync(GroupNudgeEvent e);
    Task OnGroupFileUploadAsync(GroupFileUploadEvent e);
}
