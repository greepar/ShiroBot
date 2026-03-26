using ShiroBot.Core;
using ShiroBot.Model.Common;
using ShiroBot.SDK.Adapter;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting;

internal sealed class HostEventDispatcher(Lock pluginLifecycleLock)
{
    private readonly List<LoadedPluginHandle> _groupMessageBroadcastHandlers = [];
    private readonly Dictionary<string, List<LoadedPluginHandle>> _groupMessageExactHandlers = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<(string Prefix, LoadedPluginHandle Plugin)> _groupMessagePrefixHandlers = [];
    private readonly List<LoadedPluginHandle> _friendMessageBroadcastHandlers = [];
    private readonly Dictionary<string, List<LoadedPluginHandle>> _friendMessageExactHandlers = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<(string Prefix, LoadedPluginHandle Plugin)> _friendMessagePrefixHandlers = [];
    private readonly List<LoadedPluginHandle> _messageRecallHandlers = [];
    private readonly List<LoadedPluginHandle> _friendRequestHandlers = [];
    private readonly List<LoadedPluginHandle> _groupJoinRequestHandlers = [];
    private readonly List<LoadedPluginHandle> _groupInvitedJoinRequestHandlers = [];
    private readonly List<LoadedPluginHandle> _groupInvitationHandlers = [];
    private readonly List<LoadedPluginHandle> _friendNudgeHandlers = [];
    private readonly List<LoadedPluginHandle> _friendFileUploadHandlers = [];
    private readonly List<LoadedPluginHandle> _groupAdminChangeHandlers = [];
    private readonly List<LoadedPluginHandle> _groupEssenceMessageChangeHandlers = [];
    private readonly List<LoadedPluginHandle> _groupMemberIncreaseHandlers = [];
    private readonly List<LoadedPluginHandle> _groupMemberDecreaseHandlers = [];
    private readonly List<LoadedPluginHandle> _groupNameChangeHandlers = [];
    private readonly List<LoadedPluginHandle> _groupMessageReactionHandlers = [];
    private readonly List<LoadedPluginHandle> _groupMuteHandlers = [];
    private readonly List<LoadedPluginHandle> _groupWholeMuteHandlers = [];
    private readonly List<LoadedPluginHandle> _groupNudgeHandlers = [];
    private readonly List<LoadedPluginHandle> _groupFileUploadHandlers = [];

    public void RegisterPlugin(LoadedPluginHandle pluginHandle)
    {
        lock (pluginLifecycleLock)
        {
            RegisterMessageRoutes(
                pluginHandle,
                BotEventSubscriptions.GroupMessage,
                pluginHandle.GroupMessageRoutes,
                pluginHandle.HandlesGroupMessagesViaBroadcast,
                _groupMessageBroadcastHandlers,
                _groupMessageExactHandlers,
                _groupMessagePrefixHandlers);

            RegisterMessageRoutes(
                pluginHandle,
                BotEventSubscriptions.FriendMessage,
                pluginHandle.FriendMessageRoutes,
                pluginHandle.HandlesFriendMessagesViaBroadcast,
                _friendMessageBroadcastHandlers,
                _friendMessageExactHandlers,
                _friendMessagePrefixHandlers);

            Register(pluginHandle, BotEventSubscriptions.MessageRecall, _messageRecallHandlers);
            Register(pluginHandle, BotEventSubscriptions.FriendRequest, _friendRequestHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupJoinRequest, _groupJoinRequestHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupInvitedJoinRequest, _groupInvitedJoinRequestHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupInvitation, _groupInvitationHandlers);
            Register(pluginHandle, BotEventSubscriptions.FriendNudge, _friendNudgeHandlers);
            Register(pluginHandle, BotEventSubscriptions.FriendFileUpload, _friendFileUploadHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupAdminChange, _groupAdminChangeHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupEssenceMessageChange, _groupEssenceMessageChangeHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupMemberIncrease, _groupMemberIncreaseHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupMemberDecrease, _groupMemberDecreaseHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupNameChange, _groupNameChangeHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupMessageReaction, _groupMessageReactionHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupMute, _groupMuteHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupWholeMute, _groupWholeMuteHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupNudge, _groupNudgeHandlers);
            Register(pluginHandle, BotEventSubscriptions.GroupFileUpload, _groupFileUploadHandlers);
        }
    }

    public void UnregisterPlugin(LoadedPluginHandle pluginHandle)
    {
        lock (pluginLifecycleLock)
        {
            _groupMessageBroadcastHandlers.Remove(pluginHandle);
            _friendMessageBroadcastHandlers.Remove(pluginHandle);
            RemoveRoutes(_groupMessageExactHandlers, _groupMessagePrefixHandlers, pluginHandle);
            RemoveRoutes(_friendMessageExactHandlers, _friendMessagePrefixHandlers, pluginHandle);
            _messageRecallHandlers.Remove(pluginHandle);
            _friendRequestHandlers.Remove(pluginHandle);
            _groupJoinRequestHandlers.Remove(pluginHandle);
            _groupInvitedJoinRequestHandlers.Remove(pluginHandle);
            _groupInvitationHandlers.Remove(pluginHandle);
            _friendNudgeHandlers.Remove(pluginHandle);
            _friendFileUploadHandlers.Remove(pluginHandle);
            _groupAdminChangeHandlers.Remove(pluginHandle);
            _groupEssenceMessageChangeHandlers.Remove(pluginHandle);
            _groupMemberIncreaseHandlers.Remove(pluginHandle);
            _groupMemberDecreaseHandlers.Remove(pluginHandle);
            _groupNameChangeHandlers.Remove(pluginHandle);
            _groupMessageReactionHandlers.Remove(pluginHandle);
            _groupMuteHandlers.Remove(pluginHandle);
            _groupWholeMuteHandlers.Remove(pluginHandle);
            _groupNudgeHandlers.Remove(pluginHandle);
            _groupFileUploadHandlers.Remove(pluginHandle);
        }
    }

    public Task PublishGroupMessageAsync(GroupIncomingMessage message) =>
        DispatchAsync("群消息", message, MatchGroupMessageHandlers(message), handler => handler.OnGroupMessageAsync(message));

    public Task PublishFriendMessageAsync(FriendIncomingMessage message) =>
        DispatchAsync("好友消息", message, MatchFriendMessageHandlers(message), handler => handler.OnFriendMessageAsync(message));

    public Task PublishMessageRecallAsync(MessageRecallEvent message) =>
        DispatchAsync("消息撤回", message, Snapshot(_messageRecallHandlers), handler => handler.OnMessageRecallAsync(message));

    public Task PublishFriendRequestAsync(FriendRequestEvent message) =>
        DispatchAsync("好友请求", message, Snapshot(_friendRequestHandlers), handler => handler.OnFriendRequestAsync(message));

    public Task PublishGroupJoinRequestAsync(GroupJoinRequestEvent message) =>
        DispatchAsync("入群请求", message, Snapshot(_groupJoinRequestHandlers), handler => handler.OnGroupJoinRequestAsync(message));

    public Task PublishGroupInvitedJoinRequestAsync(GroupInvitedJoinRequestEvent message) =>
        DispatchAsync("群成员邀请他人入群请求", message, Snapshot(_groupInvitedJoinRequestHandlers), handler => handler.OnGroupInvitedJoinRequestAsync(message));

    public Task PublishGroupInvitationAsync(GroupInvitationEvent message) =>
        DispatchAsync("他人邀请自身入群", message, Snapshot(_groupInvitationHandlers), handler => handler.OnGroupInvitationAsync(message));

    public Task PublishFriendNudgeAsync(FriendNudgeEvent message) =>
        DispatchAsync("好友戳一戳", message, Snapshot(_friendNudgeHandlers), handler => handler.OnFriendNudgeAsync(message));

    public Task PublishFriendFileUploadAsync(FriendFileUploadEvent message) =>
        DispatchAsync("好友文件上传", message, Snapshot(_friendFileUploadHandlers), handler => handler.OnFriendFileUploadAsync(message));

    public Task PublishGroupAdminChangeAsync(GroupAdminChangeEvent message) =>
        DispatchAsync("群管理员变更", message, Snapshot(_groupAdminChangeHandlers), handler => handler.OnGroupAdminChangeAsync(message));

    public Task PublishGroupEssenceMessageChangeAsync(GroupEssenceMessageChangeEvent message) =>
        DispatchAsync("群精华消息变更", message, Snapshot(_groupEssenceMessageChangeHandlers), handler => handler.OnGroupEssenceMessageChangeAsync(message));

    public Task PublishGroupMemberIncreaseAsync(GroupMemberIncreaseEvent message) =>
        DispatchAsync("群成员增加", message, Snapshot(_groupMemberIncreaseHandlers), handler => handler.OnGroupMemberIncreaseAsync(message));

    public Task PublishGroupMemberDecreaseAsync(GroupMemberDecreaseEvent message) =>
        DispatchAsync("群成员减少", message, Snapshot(_groupMemberDecreaseHandlers), handler => handler.OnGroupMemberDecreaseAsync(message));

    public Task PublishGroupNameChangeAsync(GroupNameChangeEvent message) =>
        DispatchAsync("群名称变更", message, Snapshot(_groupNameChangeHandlers), handler => handler.OnGroupNameChangeAsync(message));

    public Task PublishGroupMessageReactionAsync(GroupMessageReactionEvent message) =>
        DispatchAsync("群消息表情回应", message, Snapshot(_groupMessageReactionHandlers), handler => handler.OnGroupMessageReactionAsync(message));

    public Task PublishGroupMuteAsync(GroupMuteEvent message) =>
        DispatchAsync("群禁言", message, Snapshot(_groupMuteHandlers), handler => handler.OnGroupMuteAsync(message));

    public Task PublishGroupWholeMuteAsync(GroupWholeMuteEvent message) =>
        DispatchAsync("群全体禁言", message, Snapshot(_groupWholeMuteHandlers), handler => handler.OnGroupWholeMuteAsync(message));

    public Task PublishGroupNudgeAsync(GroupNudgeEvent message) =>
        DispatchAsync("群戳一戳", message, Snapshot(_groupNudgeHandlers), handler => handler.OnGroupNudgeAsync(message));

    public Task PublishGroupFileUploadAsync(GroupFileUploadEvent message) =>
        DispatchAsync("群文件上传", message, Snapshot(_groupFileUploadHandlers), handler => handler.OnGroupFileUploadAsync(message));

    private static void Register(LoadedPluginHandle pluginHandle, BotEventSubscriptions subscription, List<LoadedPluginHandle> bucket)
    {
        if ((pluginHandle.Subscriptions & subscription) != 0)
        {
            bucket.Add(pluginHandle);
        }
    }

    private static void RegisterMessageRoutes(
        LoadedPluginHandle pluginHandle,
        BotEventSubscriptions subscription,
        IReadOnlyList<MessageRouteDescriptor> routes,
        bool requiresBroadcast,
        List<LoadedPluginHandle> broadcastBucket,
        Dictionary<string, List<LoadedPluginHandle>> exactBucket,
        List<(string Prefix, LoadedPluginHandle Plugin)> prefixBucket)
    {
        if ((pluginHandle.Subscriptions & subscription) == 0)
        {
            return;
        }

        if (requiresBroadcast)
        {
            broadcastBucket.Add(pluginHandle);
        }

        foreach (var route in routes)
        {
            switch (route.MatchType)
            {
                case MessageRouteMatchType.All:
                    broadcastBucket.Add(pluginHandle);
                    break;
                case MessageRouteMatchType.Exact:
                    RegisterExact(exactBucket, route.Pattern!, pluginHandle);
                    break;
                case MessageRouteMatchType.Prefix:
                    prefixBucket.Add((route.Pattern!, pluginHandle));
                    break;
            }
        }

        if (!requiresBroadcast && routes.Count == 0)
        {
            broadcastBucket.Add(pluginHandle);
        }
    }

    private static void RegisterExact(
        Dictionary<string, List<LoadedPluginHandle>> bucket,
        string key,
        LoadedPluginHandle pluginHandle)
    {
        if (!bucket.TryGetValue(key, out var handlers))
        {
            handlers = [];
            bucket[key] = handlers;
        }

        handlers.Add(pluginHandle);
    }

    private static void RemoveRoutes(
        Dictionary<string, List<LoadedPluginHandle>> exactBucket,
        List<(string Prefix, LoadedPluginHandle Plugin)> prefixBucket,
        LoadedPluginHandle pluginHandle)
    {
        foreach (var key in exactBucket.Keys.ToArray())
        {
            exactBucket[key].Remove(pluginHandle);
            if (exactBucket[key].Count == 0)
            {
                exactBucket.Remove(key);
            }
        }

        prefixBucket.RemoveAll(entry => ReferenceEquals(entry.Plugin, pluginHandle));
    }

    private LoadedPluginHandle[] Snapshot(List<LoadedPluginHandle> bucket)
    {
        lock (pluginLifecycleLock)
        {
            return bucket.ToArray();
        }
    }

    private LoadedPluginHandle[] MatchGroupMessageHandlers(GroupIncomingMessage message)
    {
        lock (pluginLifecycleLock)
        {
            return MatchMessageHandlers(message.GetPlainText().Trim(), _groupMessageBroadcastHandlers, _groupMessageExactHandlers, _groupMessagePrefixHandlers);
        }
    }

    private LoadedPluginHandle[] MatchFriendMessageHandlers(FriendIncomingMessage message)
    {
        lock (pluginLifecycleLock)
        {
            return MatchMessageHandlers(message.GetPlainText().Trim(), _friendMessageBroadcastHandlers, _friendMessageExactHandlers, _friendMessagePrefixHandlers);
        }
    }

    private static LoadedPluginHandle[] MatchMessageHandlers(
        string text,
        List<LoadedPluginHandle> broadcastHandlers,
        Dictionary<string, List<LoadedPluginHandle>> exactHandlers,
        List<(string Prefix, LoadedPluginHandle Plugin)> prefixHandlers)
    {
        var matches = new HashSet<LoadedPluginHandle>(broadcastHandlers);

        if (exactHandlers.TryGetValue(text, out var exactMatchHandlers))
        {
            foreach (var handler in exactMatchHandlers)
            {
                matches.Add(handler);
            }
        }

        foreach (var (prefix, plugin) in prefixHandlers)
        {
            if (text.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                matches.Add(plugin);
            }
        }

        return matches.ToArray();
    }

    private async Task DispatchAsync<TEvent>(
        string eventName,
        TEvent message,
        IReadOnlyList<LoadedPluginHandle> handlers,
        Func<IBotEventSubscriber, Task> dispatch)
    {
        ConsoleHelper.Log($"收到{eventName} {Describe(message)}");

        if (handlers.Count == 0)
        {
            return;
        }

        var groupId = ExtractGroupId(message);
        var tasks = handlers
            .Where(plugin => plugin.AllowsGroup(groupId))
            .Select(async plugin =>
        {
            try
            {
                await plugin.DispatchAsync(dispatch);
            }
            catch (Exception ex)
            {
                ConsoleHelper.Error($"插件事件分发失败: {plugin.Name} - {ex.Message}");
            }
        });

        await Task.WhenAll(tasks);
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

    private static string Describe<TEvent>(TEvent evt)
    {
        return evt switch
        {
            GroupIncomingMessage message => $"{message.Group.GroupName}({message.Group.GroupId}) {message.SenderId}发送: {GetMessageSegments(message.Segments)}",
            FriendIncomingMessage message => $"{message.Friend.Nickname}({message.SenderId})发送: {GetMessageSegments(message.Segments)}",
            GroupJoinRequestEvent e => $"用户 {e.InitiatorId} 申请加入群 {e.GroupId}: {e.Comment}",
            GroupInvitedJoinRequestEvent e => $"用户 {e.InitiatorId} 邀请 {e.TargetUserId} 加入群 {e.GroupId}",
            GroupInvitationEvent e => $"用户 {e.InitiatorId} 邀请机器人加入群 {e.GroupId}",
            FriendRequestEvent e => $"用户 {e.InitiatorId} 发来好友请求: {e.Comment}",
            FriendNudgeEvent e => $"好友 {e.UserId} 发来戳一戳",
            FriendFileUploadEvent e => $"好友 {e.UserId} 上传文件: {e.FileName} ({e.FileSize} bytes)",
            GroupAdminChangeEvent e => $"{e.GroupId} 群内用户 {e.UserId}" + (e.IsSet ? " 被设为管理员" : " 被取消管理员"),
            GroupEssenceMessageChangeEvent e => $"{e.GroupId} 群消息 {e.MessageSeq}" + (e.IsSet ? " 被设为精华" : " 被取消精华"),
            GroupMemberIncreaseEvent e => $"用户 {e.UserId} 加入群 {e.GroupId}",
            GroupMemberDecreaseEvent e => $"用户 {e.UserId} 离开群 {e.GroupId}",
            GroupNameChangeEvent e => $"群 {e.GroupId} 改名为: {e.NewGroupName}",
            GroupMessageReactionEvent e => $"用户 {e.UserId} 对群 {e.GroupId} 的消息 {e.MessageSeq}" + (e.IsAdd ? $" 添加表情 {e.FaceId}" : $" 取消表情 {e.FaceId}"),
            GroupMuteEvent e => e.Duration == 0 ? $"群 {e.GroupId} 中用户 {e.UserId} 被解除禁言" : $"群 {e.GroupId} 中用户 {e.UserId} 被禁言 {e.Duration} 秒",
            GroupWholeMuteEvent e => e.IsMute ? $"群 {e.GroupId} 开启全体禁言" : $"群 {e.GroupId} 关闭全体禁言",
            GroupNudgeEvent e => $"群 {e.GroupId} 中 {e.SenderId} 戳了 {e.ReceiverId} 一下",
            GroupFileUploadEvent e => $"群 {e.GroupId} 中用户 {e.UserId} 上传文件: {e.FileName} ({e.FileSize} bytes)",
            MessageRecallEvent e => $"{GetMessageSceneName(e.MessageScene)} {e.PeerId} 中 {e.SenderId} 的消息被撤回",
            _ => string.Empty
        };
    }

    private static string GetMessageSegments(IReadOnlyList<IncomingSegment> segments)
    {
        var parts = segments.Select(segment => segment switch
        {
            TextIncomingSegment text => text.Text,
            ImageIncomingSegment image => $"[图片: {image.TempUrl}]",
            VideoIncomingSegment video => $"[视频: {video.TempUrl}]",
            RecordIncomingSegment record => $"[语音: {record.TempUrl}]",
            FileIncomingSegment file => $"[文件: {file.FileName}]",
            MentionIncomingSegment mention => $"[@{mention.UserId}]",
            MentionAllIncomingSegment => "[@全体成员]",
            FaceIncomingSegment face => $"[表情: {face.FaceId}]",
            MarketFaceIncomingSegment face => $"[商城表情: {face.Summary} {face.Url}]",
            ReplyIncomingSegment reply => $"[回复: {reply.MessageSeq}]",
            ForwardIncomingSegment forward => $"[转发: {forward.Title}]",
            LightAppIncomingSegment app => $"[轻应用: {app.AppName}]",
            XmlIncomingSegment xml => $"[XML: {xml.ServiceId}]",
            _ => $"<{segment.GetType().Name}>"
        });

        return string.Concat(parts).Replace("\r", "\\r").Replace("\n", "\\n");
    }

    private static string GetMessageSceneName(MessageRecallEventMessageScene scene)
    {
        return scene switch
        {
            MessageRecallEventMessageScene.Friend => "好友会话",
            MessageRecallEventMessageScene.Group => "群",
            MessageRecallEventMessageScene.Temp => "临时会话",
            _ => scene.ToString()
        };
    }
}
