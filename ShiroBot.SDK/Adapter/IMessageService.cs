using ShiroBot.Model.Common;
using ShiroBot.Model.Message.Requests;
using ShiroBot.Model.Message.Responses;

namespace ShiroBot.SDK.Adapter;

public interface IMessageService
{
    Task<SendPrivateMessageResponse> SendPrivateMessageAsync(long uid, OutgoingSegment[] segments)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendPrivateMessageAsync)}'.");

    Task<SendGroupMessageResponse> SendGroupMessageAsync(long groupId, OutgoingSegment[] segments)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupMessageAsync)}'.");

    Task RecallPrivateMessageAsync(long userId, long messageSeq)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RecallPrivateMessageAsync)}'.");

    Task RecallGroupMessageAsync(RecallGroupMessageRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RecallGroupMessageAsync)}'.");

    Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetMessageAsync)}'.");

    Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetHistoryMessagesAsync)}'.");

    Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetResourceTempUrlAsync)}'.");

    Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetForwardedMessagesAsync)}'.");

    Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(MarkMessageAsReadAsync)}'.");
}
