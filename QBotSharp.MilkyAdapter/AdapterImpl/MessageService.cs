using Milky.Net.Model;

using Milky.Net.Client;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class MessageService : IMessageService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;
    public async Task<SendPrivateMessageResponse> SendPrivateMessageAsync(SendPrivateMessageRequest request)
    {
        return await Milky.Message.SendPrivateMessageAsync(request);
    }

    public async Task<SendGroupMessageResponse> SendGroupMessageAsync(SendGroupMessageRequest request)
    {
        return await Milky.Message.SendGroupMessageAsync(request);
    }

    public async Task RecallPrivateMessageAsync(RecallPrivateMessageRequest request)
    {
        await Milky.Message.RecallPrivateMessageAsync(request);
    }

    public async Task RecallGroupMessageAsync(RecallGroupMessageRequest request)
    {
        await Milky.Message.RecallGroupMessageAsync(request);
    }

    public async Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request)
    {
        return await Milky.Message.GetMessageAsync(request);
    }

    public async Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request)
    {
        return await Milky.Message.GetHistoryMessagesAsync(request);
    }

    public async Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request)
    {
        return await Milky.Message.GetResourceTempUrlAsync(request);
    }

    public async Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request)
    {
        return await Milky.Message.GetForwardedMessagesAsync(request);
    }

    public async Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request)
    {
        await Milky.Message.MarkMessageAsReadAsync(request);
    }
}