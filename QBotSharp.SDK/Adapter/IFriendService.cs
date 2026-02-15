using Milky.Net.Model;

namespace QBotSharp.SDK.Adapter;

public interface IFriendService
{
    Task SendFriendNudgeAsync(SendFriendNudgeRequest request);
    Task SendProfileLikeAsync(SendProfileLikeRequest request);
    Task DeleteFriendAsync(DeleteFriendRequest request);
    Task<GetFriendRequestsResponse> GetFriendRequestsAsync(GetFriendRequestsRequest request);
    Task AcceptFriendRequestAsync(AcceptFriendRequestRequest request);
    Task RejectFriendRequestAsync(RejectFriendRequestRequest request);
}
