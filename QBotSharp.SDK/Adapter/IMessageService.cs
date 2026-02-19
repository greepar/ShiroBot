using Milky.Net.Model;

namespace QBotSharp.SDK.Adapter;

public interface IMessageService
{
    Task<SendPrivateMessageResponse> SendPrivateMessageAsync(long uid,OutgoingSegment[] segments);
    Task<SendGroupMessageResponse> SendGroupMessageAsync(long groupId,OutgoingSegment[] segments);
    Task RecallPrivateMessageAsync(long userId,long messageSeq);
    Task RecallGroupMessageAsync(RecallGroupMessageRequest request);
    Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request);
    Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request);
    Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request);
    Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request);
    Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request);
}
