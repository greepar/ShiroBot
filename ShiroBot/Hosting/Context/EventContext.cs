using ShiroBot.Model.Common;
using ShiroBot.SDK.Abstractions;
using ShiroBot.SDK.Adapter;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting.Context;

public class EventContext(
    IEventService eventService,
    IConsoleLogger logger,
    Func<long, bool>? groupRouteFilter = null) : IEventContext
{
    private const int DisableThreshold = 5;
    private readonly Dictionary<Delegate, Delegate> _wrappedHandlers = new();
    private int _consecutiveFailures;
    private int _isDisabled;

    public event Func<GroupIncomingMessage, Task> GroupMessageReceived
    {
        add => eventService.GroupMessageReceived += Wrap(value, nameof(GroupMessageReceived));
        remove => eventService.GroupMessageReceived -= Unwrap<GroupIncomingMessage>(value);
    }

    public event Func<FriendIncomingMessage, Task> FriendMessageReceived
    {
        add => eventService.FriendMessageReceived += Wrap(value, nameof(FriendMessageReceived));
        remove => eventService.FriendMessageReceived -= Unwrap<FriendIncomingMessage>(value);
    }

    public event Func<MessageRecallEvent, Task> MessageRecall
    {
        add => eventService.MessageRecall += Wrap(value, nameof(MessageRecall));
        remove => eventService.MessageRecall -= Unwrap<MessageRecallEvent>(value);
    }

    public event Func<FriendRequestEvent, Task> FriendRequest
    {
        add => eventService.FriendRequest += Wrap(value, nameof(FriendRequest));
        remove => eventService.FriendRequest -= Unwrap<FriendRequestEvent>(value);
    }

    public event Func<GroupJoinRequestEvent, Task> GroupJoinRequest
    {
        add => eventService.GroupJoinRequest += Wrap(value, nameof(GroupJoinRequest));
        remove => eventService.GroupJoinRequest -= Unwrap<GroupJoinRequestEvent>(value);
    }

    public event Func<GroupInvitedJoinRequestEvent, Task> GroupInvitedJoinRequest
    {
        add => eventService.GroupInvitedJoinRequest += Wrap(value, nameof(GroupInvitedJoinRequest));
        remove => eventService.GroupInvitedJoinRequest -= Unwrap<GroupInvitedJoinRequestEvent>(value);
    }

    public event Func<GroupInvitationEvent, Task> GroupInvitation
    {
        add => eventService.GroupInvitation += Wrap(value, nameof(GroupInvitation));
        remove => eventService.GroupInvitation -= Unwrap<GroupInvitationEvent>(value);
    }

    public event Func<FriendNudgeEvent, Task> FriendNudge
    {
        add => eventService.FriendNudge += Wrap(value, nameof(FriendNudge));
        remove => eventService.FriendNudge -= Unwrap<FriendNudgeEvent>(value);
    }

    public event Func<FriendFileUploadEvent, Task> FriendFileUpload
    {
        add => eventService.FriendFileUpload += Wrap(value, nameof(FriendFileUpload));
        remove => eventService.FriendFileUpload -= Unwrap<FriendFileUploadEvent>(value);
    }

    public event Func<GroupAdminChangeEvent, Task> GroupAdminChange
    {
        add => eventService.GroupAdminChange += Wrap(value, nameof(GroupAdminChange));
        remove => eventService.GroupAdminChange -= Unwrap<GroupAdminChangeEvent>(value);
    }

    public event Func<GroupEssenceMessageChangeEvent, Task> GroupEssenceMessageChange
    {
        add => eventService.GroupEssenceMessageChange += Wrap(value, nameof(GroupEssenceMessageChange));
        remove => eventService.GroupEssenceMessageChange -= Unwrap<GroupEssenceMessageChangeEvent>(value);
    }

    public event Func<GroupMemberIncreaseEvent, Task> GroupMemberIncrease
    {
        add => eventService.GroupMemberIncrease += Wrap(value, nameof(GroupMemberIncrease));
        remove => eventService.GroupMemberIncrease -= Unwrap<GroupMemberIncreaseEvent>(value);
    }

    public event Func<GroupMemberDecreaseEvent, Task> GroupMemberDecrease
    {
        add => eventService.GroupMemberDecrease += Wrap(value, nameof(GroupMemberDecrease));
        remove => eventService.GroupMemberDecrease -= Unwrap<GroupMemberDecreaseEvent>(value);
    }

    public event Func<GroupNameChangeEvent, Task> GroupNameChange
    {
        add => eventService.GroupNameChange += Wrap(value, nameof(GroupNameChange));
        remove => eventService.GroupNameChange -= Unwrap<GroupNameChangeEvent>(value);
    }

    public event Func<GroupMessageReactionEvent, Task> GroupMessageReaction
    {
        add => eventService.GroupMessageReaction += Wrap(value, nameof(GroupMessageReaction));
        remove => eventService.GroupMessageReaction -= Unwrap<GroupMessageReactionEvent>(value);
    }

    public event Func<GroupMuteEvent, Task> GroupMute
    {
        add => eventService.GroupMute += Wrap(value, nameof(GroupMute));
        remove => eventService.GroupMute -= Unwrap<GroupMuteEvent>(value);
    }

    public event Func<GroupWholeMuteEvent, Task> GroupWholeMute
    {
        add => eventService.GroupWholeMute += Wrap(value, nameof(GroupWholeMute));
        remove => eventService.GroupWholeMute -= Unwrap<GroupWholeMuteEvent>(value);
    }

    public event Func<GroupNudgeEvent, Task> GroupNudge
    {
        add => eventService.GroupNudge += Wrap(value, nameof(GroupNudge));
        remove => eventService.GroupNudge -= Unwrap<GroupNudgeEvent>(value);
    }

    public event Func<GroupFileUploadEvent, Task> GroupFileUpload
    {
        add => eventService.GroupFileUpload += Wrap(value, nameof(GroupFileUpload));
        remove => eventService.GroupFileUpload -= Unwrap<GroupFileUploadEvent>(value);
    }

    private Func<TEvent, Task> Wrap<TEvent>(Func<TEvent, Task> handler, string eventName)
    {
        if (_wrappedHandlers.TryGetValue(handler, out var wrapped))
        {
            return (Func<TEvent, Task>)wrapped;
        }

        Func<TEvent, Task> safeHandler = async evt =>
        {
            if (Volatile.Read(ref _isDisabled) == 1)
            {
                return;
            }

            try
            {
                if (ShouldSkipEvent(evt))
                {
                    return;
                }

                using var _ = BotLog.BeginScope(logger);
                await handler(evt);
                Interlocked.Exchange(ref _consecutiveFailures, 0);
            }
            catch (Exception ex)
            {
                var failures = Interlocked.Increment(ref _consecutiveFailures);
                using var _ = BotLog.BeginScope(logger);
                BotLog.Error($"处理事件 {eventName} 时发生异常: {ex.Message}");

                if (failures >= DisableThreshold && Interlocked.Exchange(ref _isDisabled, 1) == 0)
                {
                    BotLog.Error($"连续失败 {failures} 次，已暂停继续接收事件。");
                }
            }
        };

        _wrappedHandlers[handler] = safeHandler;
        return safeHandler;
    }

    private bool ShouldSkipEvent<TEvent>(TEvent evt)
    {
        if (groupRouteFilter is null)
        {
            return false;
        }

        var groupId = ExtractGroupId(evt);
        return groupId.HasValue && !groupRouteFilter(groupId.Value);
    }

    private static long? ExtractGroupId<TEvent>(TEvent evt)
    {
        return evt switch
        {
            GroupIncomingMessage message => message.Group.GroupId,
            GroupJoinRequestEvent e => e.GroupId,
            GroupInvitedJoinRequestEvent e => e.GroupId,
            GroupInvitationEvent e => e.GroupId,
            GroupAdminChangeEvent e => e.GroupId,
            GroupEssenceMessageChangeEvent e => e.GroupId,
            GroupMemberIncreaseEvent e => e.GroupId,
            GroupMemberDecreaseEvent e => e.GroupId,
            GroupNameChangeEvent e => e.GroupId,
            GroupMessageReactionEvent e => e.GroupId,
            GroupMuteEvent e => e.GroupId,
            GroupWholeMuteEvent e => e.GroupId,
            GroupNudgeEvent e => e.GroupId,
            GroupFileUploadEvent e => e.GroupId,
            MessageRecallEvent e when e.MessageScene == MessageRecallEventMessageScene.Group => e.PeerId,
            _ => null
        };
    }

    private Func<TEvent, Task> Unwrap<TEvent>(Func<TEvent, Task> handler)
    {
        if (_wrappedHandlers.Remove(handler, out var wrapped))
        {
            return (Func<TEvent, Task>)wrapped;
        }

        return handler;
    }
}
