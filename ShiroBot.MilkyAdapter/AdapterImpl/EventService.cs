using Milky.Net.Client;
using MModel = Milky.Net.Model;
using ShiroBot.MilkyAdapter.Milky;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.MilkyAdapter.AdapterImpl;

public class EventService : IEventService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public event Func<GroupIncomingMessage, Task>? GroupMessageReceived
    {
        add
        {
            Milky.Events.MessageReceive += async (_, e) =>
            {
                if (e is MModel.GroupIncomingMessage groupMessage)
                {
                    await value!(ModelMapper.Convert<GroupIncomingMessage>(groupMessage));
                }
            };
        }
        remove { }
    }

    public event Func<FriendIncomingMessage, Task>? FriendMessageReceived
    {
        add
        {
            Milky.Events.MessageReceive += async (_, e) =>
            {
                if (e is MModel.FriendIncomingMessage friendMessage)
                {
                    await value!(ModelMapper.Convert<FriendIncomingMessage>(friendMessage));
                }
            };
        }
        remove { }
    }

    public event Func<MessageRecallEvent, Task>? MessageRecall
    {
        add => Milky.Events.MessageRecall += async (_, e) => await value!(ModelMapper.Convert<MessageRecallEvent>(e));
        remove { }
    }

    public event Func<FriendRequestEvent, Task>? FriendRequest
    {
        add => Milky.Events.FriendRequest += async (_, e) => await value!(ModelMapper.Convert<FriendRequestEvent>(e));
        remove { }
    }

    public event Func<GroupJoinRequestEvent, Task>? GroupJoinRequest
    {
        add => Milky.Events.GroupJoinRequest += async (_, e) => await value!(ModelMapper.Convert<GroupJoinRequestEvent>(e));
        remove { }
    }

    public event Func<GroupInvitedJoinRequestEvent, Task>? GroupInvitedJoinRequest
    {
        add => Milky.Events.GroupInvitedJoinRequest += async (_, e) => await value!(ModelMapper.Convert<GroupInvitedJoinRequestEvent>(e));
        remove { }
    }

    public event Func<GroupInvitationEvent, Task>? GroupInvitation
    {
        add => Milky.Events.GroupInvitation += async (_, e) => await value!(ModelMapper.Convert<GroupInvitationEvent>(e));
        remove { }
    }

    public event Func<FriendNudgeEvent, Task>? FriendNudge
    {
        add => Milky.Events.FriendNudge += async (_, e) => await value!(ModelMapper.Convert<FriendNudgeEvent>(e));
        remove { }
    }

    public event Func<FriendFileUploadEvent, Task>? FriendFileUpload
    {
        add => Milky.Events.FriendFileUpload += async (_, e) => await value!(ModelMapper.Convert<FriendFileUploadEvent>(e));
        remove { }
    }

    public event Func<GroupAdminChangeEvent, Task>? GroupAdminChange
    {
        add => Milky.Events.GroupAdminChange += async (_, e) => await value!(ModelMapper.Convert<GroupAdminChangeEvent>(e));
        remove { }
    }

    public event Func<GroupEssenceMessageChangeEvent, Task>? GroupEssenceMessageChange
    {
        add => Milky.Events.GroupEssenceMessageChange += async (_, e) => await value!(ModelMapper.Convert<GroupEssenceMessageChangeEvent>(e));
        remove { }
    }

    public event Func<GroupMemberIncreaseEvent, Task>? GroupMemberIncrease
    {
        add => Milky.Events.GroupMemberIncrease += async (_, e) => await value!(ModelMapper.Convert<GroupMemberIncreaseEvent>(e));
        remove { }
    }

    public event Func<GroupMemberDecreaseEvent, Task>? GroupMemberDecrease
    {
        add => Milky.Events.GroupMemberDecrease += async (_, e) => await value!(ModelMapper.Convert<GroupMemberDecreaseEvent>(e));
        remove { }
    }

    public event Func<GroupNameChangeEvent, Task>? GroupNameChange
    {
        add => Milky.Events.GroupNameChange += async (_, e) => await value!(ModelMapper.Convert<GroupNameChangeEvent>(e));
        remove { }
    }

    public event Func<GroupMessageReactionEvent, Task>? GroupMessageReaction
    {
        add => Milky.Events.GroupMessageReaction += async (_, e) => await value!(ModelMapper.Convert<GroupMessageReactionEvent>(e));
        remove { }
    }

    public event Func<GroupMuteEvent, Task>? GroupMute
    {
        add => Milky.Events.GroupMute += async (_, e) => await value!(ModelMapper.Convert<GroupMuteEvent>(e));
        remove { }
    }

    public event Func<GroupWholeMuteEvent, Task>? GroupWholeMute
    {
        add => Milky.Events.GroupWholeMute += async (_, e) => await value!(ModelMapper.Convert<GroupWholeMuteEvent>(e));
        remove { }
    }

    public event Func<GroupNudgeEvent, Task>? GroupNudge
    {
        add => Milky.Events.GroupNudge += async (_, e) => await value!(ModelMapper.Convert<GroupNudgeEvent>(e));
        remove { }
    }

    public event Func<GroupFileUploadEvent, Task>? GroupFileUpload
    {
        add => Milky.Events.GroupFileUpload += async (_, e) => await value!(ModelMapper.Convert<GroupFileUploadEvent>(e));
        remove { }
    }
}
