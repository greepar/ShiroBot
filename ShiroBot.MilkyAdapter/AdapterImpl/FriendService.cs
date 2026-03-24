using Milky.Net.Client;
using MModel = Milky.Net.Model;
using ShiroBot.MilkyAdapter.Milky;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.MilkyAdapter.AdapterImpl;

public class FriendService : IFriendService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task SendFriendNudgeAsync(SendFriendNudgeRequest request) =>
        await Milky.Friend.SendFriendNudgeAsync(ModelMapper.Convert<MModel.SendFriendNudgeRequest>(request));

    public async Task SendProfileLikeAsync(SendProfileLikeRequest request) =>
        await Milky.Friend.SendProfileLikeAsync(ModelMapper.Convert<MModel.SendProfileLikeRequest>(request));

    public async Task DeleteFriendAsync(DeleteFriendRequest request) =>
        await Milky.Friend.DeleteFriendAsync(ModelMapper.Convert<MModel.DeleteFriendRequest>(request));

    public async Task<GetFriendRequestsResponse> GetFriendRequestsAsync(GetFriendRequestsRequest request) =>
        ModelMapper.Convert<GetFriendRequestsResponse>(
            await Milky.Friend.GetFriendRequestsAsync(ModelMapper.Convert<MModel.GetFriendRequestsRequest>(request)));

    public async Task AcceptFriendRequestAsync(AcceptFriendRequestRequest request) =>
        await Milky.Friend.AcceptFriendRequestAsync(ModelMapper.Convert<MModel.AcceptFriendRequestRequest>(request));

    public async Task RejectFriendRequestAsync(RejectFriendRequestRequest request) =>
        await Milky.Friend.RejectFriendRequestAsync(ModelMapper.Convert<MModel.RejectFriendRequestRequest>(request));
}
