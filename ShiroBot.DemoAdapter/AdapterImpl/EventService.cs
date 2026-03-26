using ShiroBot.SDK.Adapter;

namespace ShiroBot.DemoAdapter.AdapterImpl;

public sealed class EventService : IEventService
{
    public event Func<GroupIncomingMessage, Task>? GroupMessageReceived;
    public event Func<FriendIncomingMessage, Task>? FriendMessageReceived;
    public event Func<MessageRecallEvent, Task>? MessageRecall;
    public event Func<FriendRequestEvent, Task>? FriendRequest;
    public event Func<GroupJoinRequestEvent, Task>? GroupJoinRequest;
    public event Func<GroupInvitedJoinRequestEvent, Task>? GroupInvitedJoinRequest;
    public event Func<GroupInvitationEvent, Task>? GroupInvitation;
    public event Func<FriendNudgeEvent, Task>? FriendNudge;
    public event Func<FriendFileUploadEvent, Task>? FriendFileUpload;
    public event Func<GroupAdminChangeEvent, Task>? GroupAdminChange;
    public event Func<GroupEssenceMessageChangeEvent, Task>? GroupEssenceMessageChange;
    public event Func<GroupMemberIncreaseEvent, Task>? GroupMemberIncrease;
    public event Func<GroupMemberDecreaseEvent, Task>? GroupMemberDecrease;
    public event Func<GroupNameChangeEvent, Task>? GroupNameChange;
    public event Func<GroupMessageReactionEvent, Task>? GroupMessageReaction;
    public event Func<GroupMuteEvent, Task>? GroupMute;
    public event Func<GroupWholeMuteEvent, Task>? GroupWholeMute;
    public event Func<GroupNudgeEvent, Task>? GroupNudge;
    public event Func<GroupFileUploadEvent, Task>? GroupFileUpload;
}
