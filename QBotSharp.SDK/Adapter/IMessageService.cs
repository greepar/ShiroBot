using QBotSharp.Model.Common;
using QBotSharp.Model.Message.Requests;
using QBotSharp.Model.Message.Responses;

namespace QBotSharp.SDK.Adapter;

public interface IMessageService
{
    Task<SendPrivateMessageResponse> SendPrivateMessageAsync(long uid, OutgoingSegment[] segments)
        => AdapterFeatureNotSupported.NotSupportedAsync<SendPrivateMessageResponse>(nameof(SendPrivateMessageAsync));

    Task<SendGroupMessageResponse> SendGroupMessageAsync(long groupId, OutgoingSegment[] segments)
        => AdapterFeatureNotSupported.NotSupportedAsync<SendGroupMessageResponse>(nameof(SendGroupMessageAsync));

    Task RecallPrivateMessageAsync(long userId, long messageSeq)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(RecallPrivateMessageAsync));

    Task RecallGroupMessageAsync(RecallGroupMessageRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(RecallGroupMessageAsync));

    Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetMessageResponse>(nameof(GetMessageAsync));

    Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetHistoryMessagesResponse>(nameof(GetHistoryMessagesAsync));

    Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetResourceTempUrlResponse>(nameof(GetResourceTempUrlAsync));

    Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetForwardedMessagesResponse>(nameof(GetForwardedMessagesAsync));

    Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(MarkMessageAsReadAsync));
}
