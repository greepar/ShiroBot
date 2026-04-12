using ShiroBot.Model.Friend.Requests;
using ShiroBot.Model.Friend.Responses;

namespace ShiroBot.SDK.Adapter;

public interface IFriendService
{
    Task SendFriendNudgeAsync(SendFriendNudgeRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendFriendNudgeAsync)}'.");

    Task SendProfileLikeAsync(SendProfileLikeRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendProfileLikeAsync)}'.");

    Task DeleteFriendAsync(DeleteFriendRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(DeleteFriendAsync)}'.");

    Task<GetFriendRequestsResponse> GetFriendRequestsAsync(GetFriendRequestsRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetFriendRequestsAsync)}'.");

    Task AcceptFriendRequestAsync(AcceptFriendRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(AcceptFriendRequestAsync)}'.");

    Task RejectFriendRequestAsync(RejectFriendRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RejectFriendRequestAsync)}'.");
}
