using QBotSharp.Core;
using QBotSharp.Model.System.Requests;
using QBotSharp.Model.System.Responses;
using QBotSharp.SDK.Adapter;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.Hosting.Context;

public class SystemContext(ISystemService system) : ISystemContext
{
    public Task<GetLoginInfoResponse> GetLoginInfoAsync()
    {
        ConsoleHelper.Info("[Plugin -> System] Getting login info");
        return system.GetLoginInfoAsync();
    }

    public Task<GetImplInfoResponse> GetImplInfoAsync() => system.GetImplInfoAsync();
    public Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest request) => system.GetUserProfileAsync(request);
    public Task<GetFriendListResponse> GetFriendListAsync(GetFriendListRequest request) => system.GetFriendListAsync(request);
    public Task<GetFriendInfoResponse> GetFriendInfoAsync(GetFriendInfoRequest request) => system.GetFriendInfoAsync(request);
    public Task<GetGroupListResponse> GetGroupListAsync(GetGroupListRequest request) => system.GetGroupListAsync(request);
    public Task<GetGroupInfoResponse> GetGroupInfoAsync(GetGroupInfoRequest request) => system.GetGroupInfoAsync(request);
    public Task<GetGroupMemberListResponse> GetGroupMemberListAsync(GetGroupMemberListRequest request) => system.GetGroupMemberListAsync(request);
    public Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(GetGroupMemberInfoRequest request) => system.GetGroupMemberInfoAsync(request);
    public Task SetAvatarAsync(SetAvatarRequest request) => system.SetAvatarAsync(request);
    public Task SetNicknameAsync(SetNicknameRequest request) => system.SetNicknameAsync(request);
    public Task SetBioAsync(SetBioRequest request) => system.SetBioAsync(request);
    public Task<GetCustomFaceUrlListResponse> GetCustomFaceUrlListAsync() => system.GetCustomFaceUrlListAsync();
    public Task<GetCookiesResponse> GetCookiesAsync(GetCookiesRequest request) => system.GetCookiesAsync(request);
    public Task<GetCsrfTokenResponse> GetCsrfTokenAsync() => system.GetCsrfTokenAsync();
}
