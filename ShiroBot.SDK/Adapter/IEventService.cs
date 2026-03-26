using ShiroBot.Model.Common;

namespace ShiroBot.SDK.Adapter;

public interface IEventService
{
    event Func<GroupIncomingMessage, Task> GroupMessageReceived;
    event Func<FriendIncomingMessage, Task> FriendMessageReceived;
    event Func<MessageRecallEvent, Task> MessageRecall;
    event Func<FriendRequestEvent, Task> FriendRequest;
    event Func<GroupJoinRequestEvent, Task> GroupJoinRequest;
    event Func<GroupInvitedJoinRequestEvent, Task> GroupInvitedJoinRequest;
    event Func<GroupInvitationEvent, Task> GroupInvitation;
    event Func<FriendNudgeEvent, Task> FriendNudge;
    event Func<FriendFileUploadEvent, Task> FriendFileUpload;
    event Func<GroupAdminChangeEvent, Task> GroupAdminChange;
    event Func<GroupEssenceMessageChangeEvent, Task> GroupEssenceMessageChange;
    event Func<GroupMemberIncreaseEvent, Task> GroupMemberIncrease;
    event Func<GroupMemberDecreaseEvent, Task> GroupMemberDecrease;
    event Func<GroupNameChangeEvent, Task> GroupNameChange;
    event Func<GroupMessageReactionEvent, Task> GroupMessageReaction;
    event Func<GroupMuteEvent, Task> GroupMute;
    event Func<GroupWholeMuteEvent, Task> GroupWholeMute;
    event Func<GroupNudgeEvent, Task> GroupNudge;
    event Func<GroupFileUploadEvent, Task> GroupFileUpload;
}
