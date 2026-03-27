using ShiroBot.Model.Common;
using ShiroBot.Model.Message.Requests;
using ShiroBot.Model.Message.Responses;

namespace ShiroBot.SDK.Adapter;

public interface IMessageService
{
    Task<SendPrivateMessageResponse> SendPrivateMessageAsync(long uid, OutgoingSegment[] segments)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendPrivateMessageAsync)}'.");

    Task<SendPrivateMessageResponse> SendPrivateMessageAsync(SendPrivateMessageRequest request)
        => SendPrivateMessageAsync(request.UserId, [.. request.Message]);

    Task<SendGroupMessageResponse> SendGroupMessageAsync(long groupId, OutgoingSegment[] segments)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupMessageAsync)}'.");

    Task<SendGroupMessageResponse> SendGroupMessageAsync(SendGroupMessageRequest request)
        => SendGroupMessageAsync(request.GroupId, [.. request.Message]);

    Task RecallPrivateMessageAsync(long userId, long messageSeq)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RecallPrivateMessageAsync)}'.");

    Task RecallPrivateMessageAsync(RecallPrivateMessageRequest request)
        => RecallPrivateMessageAsync(request.UserId, request.MessageSeq);

    Task RecallGroupMessageAsync(RecallGroupMessageRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RecallGroupMessageAsync)}'.");

    Task RecallGroupMessageAsync(long groupId, long messageSeq)
        => RecallGroupMessageAsync(new RecallGroupMessageRequest(groupId, messageSeq));

    Task<GetMessageResponse> GetMessageAsync(GetMessageRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetMessageAsync)}'.");

    Task<GetMessageResponse> GetMessageAsync(GetMessageRequestMessageScene messageScene, long peerId, long messageSeq)
        => GetMessageAsync(new GetMessageRequest(messageScene, peerId, messageSeq));

    Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetHistoryMessagesAsync)}'.");

    Task<GetHistoryMessagesResponse> GetHistoryMessagesAsync(GetHistoryMessagesRequestMessageScene messageScene, long peerId, long? startMessageSeq = null, int limit = 20)
        => GetHistoryMessagesAsync(new GetHistoryMessagesRequest(messageScene, peerId, startMessageSeq, limit));

    Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(GetResourceTempUrlRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetResourceTempUrlAsync)}'.");

    Task<GetResourceTempUrlResponse> GetResourceTempUrlAsync(string resourceId)
        => GetResourceTempUrlAsync(new GetResourceTempUrlRequest(resourceId));

    Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(GetForwardedMessagesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetForwardedMessagesAsync)}'.");

    Task<GetForwardedMessagesResponse> GetForwardedMessagesAsync(string forwardId)
        => GetForwardedMessagesAsync(new GetForwardedMessagesRequest(forwardId));

    Task MarkMessageAsReadAsync(MarkMessageAsReadRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(MarkMessageAsReadAsync)}'.");

    Task MarkMessageAsReadAsync(MarkMessageAsReadRequestMessageScene messageScene, long peerId, long messageSeq)
        => MarkMessageAsReadAsync(new MarkMessageAsReadRequest(messageScene, peerId, messageSeq));
}
