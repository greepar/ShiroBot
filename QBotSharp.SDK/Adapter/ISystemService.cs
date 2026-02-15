using Milky.Net.Model;

namespace QBotSharp.SDK.Adapter;

public interface ISystemService
{
    Task<GetLoginInfoResponse> GetLoginInfoAsync();
    Task<GetImplInfoResponse> GetImplInfoAsync();
    Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest request);
    Task<GetFriendListResponse> GetFriendListAsync(GetFriendListRequest request);
    Task<GetFriendInfoResponse> GetFriendInfoAsync(GetFriendInfoRequest request);
    Task DeleteFriendAsync(DeleteFriendRequest request);
    Task<GetGroupListResponse> GetGroupListAsync(GetGroupListRequest request);
    Task<GetGroupInfoResponse> GetGroupInfoAsync(GetGroupInfoRequest request);
    Task<GetGroupMemberListResponse> GetGroupMemberListAsync(GetGroupMemberListRequest request);
    Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(GetGroupMemberInfoRequest request);
    Task SetAvatarAsync(SetAvatarRequest request);
    Task SetNicknameAsync(SetNicknameRequest request);
    Task SetBioAsync(SetBioRequest request);
    Task<GetCustomFaceUrlListResponse> GetCustomFaceUrlListAsync();
    Task<GetCookiesResponse> GetCookiesAsync(GetCookiesRequest request);
    Task<GetCsrfTokenResponse> GetCsrfTokenAsync();
}
