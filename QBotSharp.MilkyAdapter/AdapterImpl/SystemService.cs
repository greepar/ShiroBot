using Milky.Net.Client;
using MModel = Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class SystemService : ISystemService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task<GetLoginInfoResponse> GetLoginInfoAsync() =>
        ModelMapper.Convert<GetLoginInfoResponse>(await Milky.System.GetLoginInfoAsync());

    public async Task<GetImplInfoResponse> GetImplInfoAsync() =>
        ModelMapper.Convert<GetImplInfoResponse>(await Milky.System.GetImplInfoAsync());

    public async Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest request) =>
        ModelMapper.Convert<GetUserProfileResponse>(
            await Milky.System.GetUserProfileAsync(ModelMapper.Convert<MModel.GetUserProfileRequest>(request)));

    public async Task<GetFriendListResponse> GetFriendListAsync(GetFriendListRequest request) =>
        ModelMapper.Convert<GetFriendListResponse>(
            await Milky.System.GetFriendListAsync(ModelMapper.Convert<MModel.GetFriendListRequest>(request)));

    public async Task<GetFriendInfoResponse> GetFriendInfoAsync(GetFriendInfoRequest request) =>
        ModelMapper.Convert<GetFriendInfoResponse>(
            await Milky.System.GetFriendInfoAsync(ModelMapper.Convert<MModel.GetFriendInfoRequest>(request)));

    public async Task DeleteFriendAsync(DeleteFriendRequest request) =>
        await Milky.Friend.DeleteFriendAsync(ModelMapper.Convert<MModel.DeleteFriendRequest>(request));

    public async Task<GetGroupListResponse> GetGroupListAsync(GetGroupListRequest request) =>
        ModelMapper.Convert<GetGroupListResponse>(
            await Milky.System.GetGroupListAsync(ModelMapper.Convert<MModel.GetGroupListRequest>(request)));

    public async Task<GetGroupInfoResponse> GetGroupInfoAsync(GetGroupInfoRequest request) =>
        ModelMapper.Convert<GetGroupInfoResponse>(
            await Milky.System.GetGroupInfoAsync(ModelMapper.Convert<MModel.GetGroupInfoRequest>(request)));

    public async Task<GetGroupMemberListResponse> GetGroupMemberListAsync(GetGroupMemberListRequest request) =>
        ModelMapper.Convert<GetGroupMemberListResponse>(
            await Milky.System.GetGroupMemberListAsync(ModelMapper.Convert<MModel.GetGroupMemberListRequest>(request)));

    public async Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(GetGroupMemberInfoRequest request) =>
        ModelMapper.Convert<GetGroupMemberInfoResponse>(
            await Milky.System.GetGroupMemberInfoAsync(ModelMapper.Convert<MModel.GetGroupMemberInfoRequest>(request)));

    public async Task SetAvatarAsync(SetAvatarRequest request) =>
        await Milky.System.SetAvatarAsync(ModelMapper.Convert<MModel.SetAvatarRequest>(request));

    public async Task SetNicknameAsync(SetNicknameRequest request) =>
        await Milky.System.SetNicknameAsync(ModelMapper.Convert<MModel.SetNicknameRequest>(request));

    public async Task SetBioAsync(SetBioRequest request) =>
        await Milky.System.SetBioAsync(ModelMapper.Convert<MModel.SetBioRequest>(request));

    public async Task<GetCustomFaceUrlListResponse> GetCustomFaceUrlListAsync() =>
        ModelMapper.Convert<GetCustomFaceUrlListResponse>(await Milky.System.GetCustomFaceUrlListAsync());

    public async Task<GetCookiesResponse> GetCookiesAsync(GetCookiesRequest request) =>
        ModelMapper.Convert<GetCookiesResponse>(
            await Milky.System.GetCookiesAsync(ModelMapper.Convert<MModel.GetCookiesRequest>(request)));

    public async Task<GetCsrfTokenResponse> GetCsrfTokenAsync() =>
        ModelMapper.Convert<GetCsrfTokenResponse>(await Milky.System.GetCsrfTokenAsync());
}
