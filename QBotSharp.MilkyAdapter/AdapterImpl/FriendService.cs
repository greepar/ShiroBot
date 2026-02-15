using Milky.Net.Client;
using Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class FriendService : IFriendService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task SendFriendNudgeAsync(SendFriendNudgeRequest request)
    {
        await Milky.Friend.SendFriendNudgeAsync(request);
    }

    public async Task SendProfileLikeAsync(SendProfileLikeRequest request)
    {
        await Milky.Friend.SendProfileLikeAsync(request);
    }

    public async Task DeleteFriendAsync(DeleteFriendRequest request)
    {
        await Milky.Friend.DeleteFriendAsync(request);
    }

    public async Task<GetFriendRequestsResponse> GetFriendRequestsAsync(GetFriendRequestsRequest request)
    {
        return await Milky.Friend.GetFriendRequestsAsync(request);
    }

    public async Task AcceptFriendRequestAsync(AcceptFriendRequestRequest request)
    {
        await Milky.Friend.AcceptFriendRequestAsync(request);
    }

    public async Task RejectFriendRequestAsync(RejectFriendRequestRequest request)
    {
        await Milky.Friend.RejectFriendRequestAsync(request);
    }
}