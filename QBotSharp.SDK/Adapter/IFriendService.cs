using QBotSharp.Model.Friend.Requests;
using QBotSharp.Model.Friend.Responses;

namespace QBotSharp.SDK.Adapter;

public interface IFriendService
{
    Task SendFriendNudgeAsync(SendFriendNudgeRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SendFriendNudgeAsync));

    Task SendProfileLikeAsync(SendProfileLikeRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SendProfileLikeAsync));

    Task DeleteFriendAsync(DeleteFriendRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(DeleteFriendAsync));

    Task<GetFriendRequestsResponse> GetFriendRequestsAsync(GetFriendRequestsRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetFriendRequestsResponse>(nameof(GetFriendRequestsAsync));

    Task AcceptFriendRequestAsync(AcceptFriendRequestRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(AcceptFriendRequestAsync));

    Task RejectFriendRequestAsync(RejectFriendRequestRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(RejectFriendRequestAsync));
}
