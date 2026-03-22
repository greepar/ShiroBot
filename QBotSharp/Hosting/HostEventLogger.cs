using QBotSharp.Core;
using QBotSharp.Model.Common;
using QBotSharp.SDK.Adapter;
using QBotSharp.Utils;

namespace QBotSharp.Hosting;

internal static class HostEventLogger
{
    public static void Attach(IEventService eventService)
    {
        eventService.GroupMessageReceived += e => Log("群消息", e);
        eventService.FriendMessageReceived += e => Log("好友消息", e);
        eventService.MessageRecall += e => Log("消息撤回", e);
        eventService.FriendRequest += e => Log("好友请求", e);
        eventService.GroupJoinRequest += e => Log("入群请求", e);
        eventService.GroupInvitedJoinRequest += e => Log("群成员邀请他人入群请求", e);
        eventService.GroupInvitation += e => Log("他人邀请自身入群", e);
        eventService.FriendNudge += e => Log("好友戳一戳", e);
        eventService.FriendFileUpload += e => Log("好友文件上传", e);
        eventService.GroupAdminChange += e => Log("群管理员变更", e);
        eventService.GroupEssenceMessageChange += e => Log("群精华消息变更", e);
        eventService.GroupMemberIncrease += e => Log("群成员增加", e);
        eventService.GroupMemberDecrease += e => Log("群成员减少", e);
        eventService.GroupNameChange += e => Log("群名称变更", e);
        eventService.GroupMessageReaction += e => Log("群消息表情回应", e);
        eventService.GroupMute += e => Log("群禁言", e);
        eventService.GroupWholeMute += e => Log("群全体禁言", e);
        eventService.GroupNudge += e => Log("群戳一戳", e);
        eventService.GroupFileUpload += e => Log("群文件上传", e);
    }

    private static Task Log<TEvent>(string eventName, TEvent evt)
    {
        ConsoleHelper.Log($"收到{eventName} {Describe(evt)}");
        return Task.CompletedTask;
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
            GroupMuteEvent e => e.Duration == 0
                ? $"群 {e.GroupId} 中用户 {e.UserId} 被解除禁言"
                : $"群 {e.GroupId} 中用户 {e.UserId} 被禁言 {e.Duration} 秒",
            GroupWholeMuteEvent e => e.IsMute
                ? $"群 {e.GroupId} 开启全体禁言"
                : $"群 {e.GroupId} 关闭全体禁言",
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
