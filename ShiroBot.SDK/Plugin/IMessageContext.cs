using ShiroBot.Model.Common;
using ShiroBot.Model.Message.Responses;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.SDK.Plugin;

public interface IMessageContext : IMessageService
{
    Task<SendPrivateMessageResponse> SendPrivateMessageAsync(
        long userId,
        string text,
        params OutgoingSegment[] additionalSegments) =>
        SendPrivateMessageAsync(userId, BuildSegments(text, additionalSegments));

    Task<SendGroupMessageResponse> SendGroupMessageAsync(
        long groupId,
        string text,
        params OutgoingSegment[] additionalSegments) =>
        SendGroupMessageAsync(groupId, BuildSegments(text, additionalSegments));

    Task<SendPrivateMessageResponse> ReplyAsync(
        FriendIncomingMessage message,
        string text,
        params OutgoingSegment[] additionalSegments) =>
        SendPrivateMessageAsync(message.SenderId, BuildSegments(text, additionalSegments));

    Task<SendGroupMessageResponse> ReplyAsync(
        GroupIncomingMessage message,
        string text,
        params OutgoingSegment[] additionalSegments) =>
        SendGroupMessageAsync(message.Group.GroupId, BuildSegments(text, additionalSegments));

    private static OutgoingSegment[] BuildSegments(string text, IReadOnlyList<OutgoingSegment> additionalSegments)
    {
        var segments = new OutgoingSegment[additionalSegments.Count + 1];
        segments[0] = new TextOutgoingSegment(text);

        for (var i = 0; i < additionalSegments.Count; i++)
        {
            segments[i + 1] = additionalSegments[i];
        }

        return segments;
    }
}
