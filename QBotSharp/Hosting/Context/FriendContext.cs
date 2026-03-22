using QBotSharp.Core;
using QBotSharp.Model.Friend.Requests;
using QBotSharp.Model.Friend.Responses;
using QBotSharp.SDK.Adapter;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.Hosting.Context;

public class FriendContext(IFriendService friend) : IFriendContext
{
    public Task SendFriendNudgeAsync(SendFriendNudgeRequest request)
    {
        ConsoleHelper.Info($"[Plugin -> Friend] Sending nudge to {request.UserId}");
        return friend.SendFriendNudgeAsync(request);
    }
    
    public Task SendProfileLikeAsync(SendProfileLikeRequest request) =>
        friend.SendProfileLikeAsync(request);

    public Task DeleteFriendAsync(DeleteFriendRequest request) =>
        friend.DeleteFriendAsync(request);

    public Task<GetFriendRequestsResponse> GetFriendRequestsAsync(GetFriendRequestsRequest request) =>
        friend.GetFriendRequestsAsync(request);

    public Task AcceptFriendRequestAsync(AcceptFriendRequestRequest request) =>
        friend.AcceptFriendRequestAsync(request);

    public Task RejectFriendRequestAsync(RejectFriendRequestRequest request) =>
        friend.RejectFriendRequestAsync(request);
}
