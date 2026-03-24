using Milky.Net.Client;
using MModel = Milky.Net.Model;
using ShiroBot.MilkyAdapter.Milky;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.MilkyAdapter.AdapterImpl;

public class MessageService : IMessageService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task<SendPrivateMessageResponse> SendPrivateMessageAsync(long uid, OutgoingSegment[] segments)
    {
        var input = new MModel.SendPrivateMessageRequest(uid, ModelMapper.Convert<MModel.OutgoingSegment[]>(segments));
        return ModelMapper.Convert<SendPrivateMessageResponse>(await Milky.Message.SendPrivateMessageAsync(input));
    }

    public async Task<SendGroupMessageResponse> SendGroupMessageAsync(long groupId, OutgoingSegment[] segments)
    {
        var input = new MModel.SendGroupMessageRequest(groupId, ModelMapper.Convert<MModel.OutgoingSegment[]>(segments));
        return ModelMapper.Convert<SendGroupMessageResponse>(await Milky.Message.SendGroupMessageAsync(input));
    }

    public async Task RecallPrivateMessageAsync(long userId, long messageSeq)
    {
        await Milky.Message.RecallPrivateMessageAsync(new MModel.RecallPrivateMessageRequest(userId, messageSeq));
    }

    public async Task RecallGroupMessageAsync(RecallGroupMessageRequest request)
    {
        await Milky.Message.RecallGroupMessageAsync(ModelMapper.Convert<MModel.RecallGroupMessageRequest>(request));
    }

    public async Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request)
    {
        return ModelMapper.Convert<GetMessageResponse>(
            await Milky.Message.GetMessageAsync(ModelMapper.Convert<MModel.GetMessageRequest>(request)));
    }

    public async Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request)
    {
        return ModelMapper.Convert<GetHistoryMessagesResponse>(
            await Milky.Message.GetHistoryMessagesAsync(ModelMapper.Convert<MModel.GetHistoryMessagesRequest>(request)));
    }

    public async Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request)
    {
        return ModelMapper.Convert<GetResourceTempUrlResponse>(
            await Milky.Message.GetResourceTempUrlAsync(ModelMapper.Convert<MModel.GetResourceTempUrlRequest>(request)));
    }

    public async Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request)
    {
        return ModelMapper.Convert<GetForwardedMessagesResponse>(
            await Milky.Message.GetForwardedMessagesAsync(ModelMapper.Convert<MModel.GetForwardedMessagesRequest>(request)));
    }

    public async Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request)
    {
        await Milky.Message.MarkMessageAsReadAsync(ModelMapper.Convert<MModel.MarkMessageAsReadRequest>(request));
    }
}
