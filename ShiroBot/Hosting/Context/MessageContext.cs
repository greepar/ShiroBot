using ShiroBot.Model.Common;
using ShiroBot.Model.Message.Requests;
using ShiroBot.Model.Message.Responses;
using ShiroBot.SDK.Adapter;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting.Context;

public class MessageContext(IMessageService message) : IMessageContext
{
    public Task<SendPrivateMessageResponse> SendPrivateMessageAsync(SendPrivateMessageRequest request) =>
        message.SendPrivateMessageAsync(request);

    public Task<SendGroupMessageResponse> SendGroupMessageAsync(SendGroupMessageRequest request) =>
        message.SendGroupMessageAsync(request);

    public Task RecallPrivateMessageAsync(RecallPrivateMessageRequest request) =>
        message.RecallPrivateMessageAsync(request);

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
