using ShiroBot.Model.Common;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.SDK.Plugin;

public interface IMessageContext : IMessageService
{
    Task SendPrivateTextAsync(long userId, string text) =>
        SendPrivateMessageAsync(userId, [new TextOutgoingSegment(text)]);

    Task SendGroupTextAsync(long groupId, string text) =>
        SendGroupMessageAsync(groupId, [new TextOutgoingSegment(text)]);

    Task ReplyTextAsync(FriendIncomingMessage message, string text) =>
        SendPrivateTextAsync(message.SenderId, text);

    Task ReplyTextAsync(GroupIncomingMessage message, string text) =>
        SendGroupTextAsync(message.Group.GroupId, text);
}
