using Milky.Net.Model;

namespace QBotSharp.SDK.Adapter;

public interface IMessageService
{
    Task<SendPrivateMessageResponse> SendPrivateMessageAsync(SendPrivateMessageRequest request);
    Task<SendGroupMessageResponse> SendGroupMessageAsync(SendGroupMessageRequest request);
    Task RecallPrivateMessageAsync(RecallPrivateMessageRequest request);
    Task RecallGroupMessageAsync(RecallGroupMessageRequest request);
    Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request);
    Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request);
    Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request);
    Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request);
    Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request);
}
