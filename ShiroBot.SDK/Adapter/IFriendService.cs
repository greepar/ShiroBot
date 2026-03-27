using ShiroBot.Model.Friend.Requests;
using ShiroBot.Model.Friend.Responses;

namespace ShiroBot.SDK.Adapter;

public interface IFriendService
{
    Task SendFriendNudgeAsync(SendFriendNudgeRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendFriendNudgeAsync)}'.");

    Task SendFriendNudgeAsync(long userId, bool isSelf = false)
        => SendFriendNudgeAsync(new SendFriendNudgeRequest(userId, isSelf));

    Task SendProfileLikeAsync(SendProfileLikeRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendProfileLikeAsync)}'.");

    Task SendProfileLikeAsync(long userId, int count = 1)
        => SendProfileLikeAsync(new SendProfileLikeRequest(userId, count));

    Task DeleteFriendAsync(DeleteFriendRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(DeleteFriendAsync)}'.");

    Task DeleteFriendAsync(long userId)
        => DeleteFriendAsync(new DeleteFriendRequest(userId));

    Task<GetFriendRequestsResponse> GetFriendRequestsAsync(GetFriendRequestsRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetFriendRequestsAsync)}'.");

    Task<GetFriendRequestsResponse> GetFriendRequestsAsync(int limit = 20, bool isFiltered = false)
        => GetFriendRequestsAsync(new GetFriendRequestsRequest(limit, isFiltered));

    Task AcceptFriendRequestAsync(AcceptFriendRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(AcceptFriendRequestAsync)}'.");

    Task AcceptFriendRequestAsync(string initiatorUid, bool isFiltered = false)
        => AcceptFriendRequestAsync(new AcceptFriendRequestRequest(initiatorUid, isFiltered));

    Task RejectFriendRequestAsync(RejectFriendRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RejectFriendRequestAsync)}'.");

    Task RejectFriendRequestAsync(string initiatorUid, bool isFiltered = false, string? reason = null)
        => RejectFriendRequestAsync(new RejectFriendRequestRequest(initiatorUid, isFiltered, reason));
}
