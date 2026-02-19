using QBotSharp.SDK.Adapter;
using Milky.Net.Model;
using QBotSharp.SDK.Plugin;
using QBotSharp.Utils;

namespace QBotSharp.Hosting.BotContext;

public class MessageContext(IMessageService message) : IMessageContext
{
    public Task<SendPrivateMessageResponse> SendPrivateMessageAsync(long uid, OutgoingSegment[] segments)
    {
        return message.SendPrivateMessageAsync(uid, segments);
    }

    public Task<SendGroupMessageResponse> SendGroupMessageAsync(long groupId, OutgoingSegment[] segments)
    {
        return message.SendGroupMessageAsync(groupId, segments);
    }

    public Task RecallPrivateMessageAsync(long userId,long messageSeq) =>
        message.RecallPrivateMessageAsync(userId, messageSeq);

    public Task RecallGroupMessageAsync(RecallGroupMessageRequest request) =>
        message.RecallGroupMessageAsync(request);

    public Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request) =>
        message.GetMessageAsync(request);

    public Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request) =>
        message.GetHistoryMessagesAsync(request);

    public Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request) =>
        message.GetResourceTempUrlAsync(request);

    public Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request) =>
        message.GetForwardedMessagesAsync(request);

    public Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request) =>
        message.MarkMessageAsReadAsync(request);
}
