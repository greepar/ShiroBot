
using Milky.Net.Client;
using Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class SystemService : ISystemService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task<GetLoginInfoResponse> GetLoginInfoAsync()
    {
        return await Milky.System.GetLoginInfoAsync();
    }

    public async Task<GetImplInfoResponse> GetImplInfoAsync()
    {
        return await Milky.System.GetImplInfoAsync();
    }

    public async Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest request)
    {
        return await Milky.System.GetUserProfileAsync(request);
    }

    public async Task<GetFriendListResponse> GetFriendListAsync(GetFriendListRequest request)
    {
        return await Milky.System.GetFriendListAsync(request);
    }

    public async Task<GetFriendInfoResponse> GetFriendInfoAsync(GetFriendInfoRequest request)
    {
        return await Milky.System.GetFriendInfoAsync(request);
    }

    public async Task DeleteFriendAsync(DeleteFriendRequest request)
    {
        await Milky.Friend.DeleteFriendAsync(request);
    }

    public async Task<GetGroupListResponse> GetGroupListAsync(GetGroupListRequest request)
    {
        return await Milky.System.GetGroupListAsync(request);
    }

    public async Task<GetGroupInfoResponse> GetGroupInfoAsync(GetGroupInfoRequest request)
    {
        return await Milky.System.GetGroupInfoAsync(request);
    }

    public async Task<GetGroupMemberListResponse> GetGroupMemberListAsync(GetGroupMemberListRequest request)
    {
        return await Milky.System.GetGroupMemberListAsync(request);
    }

    public async Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(GetGroupMemberInfoRequest request)
    {
        return await Milky.System.GetGroupMemberInfoAsync(request);
    }

    public async Task SetAvatarAsync(SetAvatarRequest request)
    {
        await Milky.System.SetAvatarAsync(request);
    }

    public async Task SetNicknameAsync(SetNicknameRequest request)
    {
        await Milky.System.SetNicknameAsync(request);
    }

    public async Task SetBioAsync(SetBioRequest request)
    {
        await Milky.System.SetBioAsync(request);
    }

    public async Task<GetCustomFaceUrlListResponse> GetCustomFaceUrlListAsync()
    {
        return await Milky.System.GetCustomFaceUrlListAsync();
    }

    public async Task<GetCookiesResponse> GetCookiesAsync(GetCookiesRequest request)
    {
        return await Milky.System.GetCookiesAsync(request);
    }

    public async Task<GetCsrfTokenResponse> GetCsrfTokenAsync()
    {
        return await Milky.System.GetCsrfTokenAsync();
    }
}