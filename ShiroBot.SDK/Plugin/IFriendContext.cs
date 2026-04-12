using ShiroBot.Model.Friend.Requests;
using ShiroBot.Model.Friend.Responses;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.SDK.Plugin;

public interface IFriendContext : IFriendService
{
    Task SendFriendNudgeAsync(long userId, bool isSelf = false) =>
        SendFriendNudgeAsync(new SendFriendNudgeRequest(userId, isSelf));

    Task SendProfileLikeAsync(long userId, int count = 1) =>
        SendProfileLikeAsync(new SendProfileLikeRequest(userId, count));

    Task DeleteFriendAsync(long userId) =>
        DeleteFriendAsync(new DeleteFriendRequest(userId));

    Task<GetFriendRequestsResponse> GetFriendRequestsAsync(int limit = 20, bool isFiltered = false) =>
        GetFriendRequestsAsync(new GetFriendRequestsRequest(limit, isFiltered));

    Task AcceptFriendRequestAsync(string initiatorUid, bool isFiltered = false) =>
        AcceptFriendRequestAsync(new AcceptFriendRequestRequest(initiatorUid, isFiltered));

    Task RejectFriendRequestAsync(string initiatorUid, bool isFiltered = false, string? reason = null) =>
        RejectFriendRequestAsync(new RejectFriendRequestRequest(initiatorUid, isFiltered, reason));
}
