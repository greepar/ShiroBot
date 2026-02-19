using Milky.Net.Model;

using Milky.Net.Client;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class MessageService : IMessageService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;
    public async Task<SendPrivateMessageResponse> SendPrivateMessageAsync(long uid,OutgoingSegment[] segments)
    {
        var input = new SendPrivateMessageRequest(uid,segments);
        return await Milky.Message.SendPrivateMessageAsync(input);
    }

    public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long groupId,OutgoingSegment[] segments)
    {
        var input = new SendGroupMessageRequest(groupId,segments);
        return await Milky.Message.SendGroupMessageAsync(input);
    }

    public async Task RecallPrivateMessageAsync(long userId,long messageSeq)
    {
        var request = new RecallPrivateMessageRequest(userId, messageSeq);
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