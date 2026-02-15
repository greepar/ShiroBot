using QBotSharp.SDK.Adapter;
using Milky.Net.Model;
using QBotSharp.SDK.Plugin;
using QBotSharp.Utils;

namespace QBotSharp.Hosting.BotContext;

public class MessageContext(IMessageService message) : IMessageContext
{
    public async Task<SendPrivateMessageResponse> SendPrivateMessageAsync(SendPrivateMessageRequest request)
    {
        ConsoleHelper.Log($"[Plugin -> Message] Sending private message to {request.UserId}");
        var response = await message.SendPrivateMessageAsync(request);
        // ConsoleHelper.Info($"[Plugin <- Message] Response: {response?.}");
        return response;
    }

    public async Task<SendGroupMessageResponse> SendGroupMessageAsync(SendGroupMessageRequest request)
    {
        ConsoleHelper.Log($"[Plugin -> Message] Sending group message to {request.GroupId}");
        var response = await message.SendGroupMessageAsync(request);
        // ConsoleHelper.Info($"[Plugin <- Message] Response: {response?.MessageId}");
        return response;
    }

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
