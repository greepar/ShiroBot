using QBotSharp.Model.System.Requests;
using QBotSharp.Model.System.Responses;

namespace QBotSharp.SDK.Adapter;

public interface ISystemService
{
    Task<GetLoginInfoResponse> GetLoginInfoAsync()
        => AdapterFeatureNotSupported.NotSupportedAsync<GetLoginInfoResponse>(nameof(GetLoginInfoAsync));

    Task<GetImplInfoResponse> GetImplInfoAsync()
        => AdapterFeatureNotSupported.NotSupportedAsync<GetImplInfoResponse>(nameof(GetImplInfoAsync));

    Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetUserProfileResponse>(nameof(GetUserProfileAsync));

    Task<GetFriendListResponse> GetFriendListAsync(GetFriendListRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetFriendListResponse>(nameof(GetFriendListAsync));

    Task<GetFriendInfoResponse> GetFriendInfoAsync(GetFriendInfoRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetFriendInfoResponse>(nameof(GetFriendInfoAsync));

    Task<GetGroupListResponse> GetGroupListAsync(GetGroupListRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupListResponse>(nameof(GetGroupListAsync));

    Task<GetGroupInfoResponse> GetGroupInfoAsync(GetGroupInfoRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupInfoResponse>(nameof(GetGroupInfoAsync));

    Task<GetGroupMemberListResponse> GetGroupMemberListAsync(GetGroupMemberListRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupMemberListResponse>(nameof(GetGroupMemberListAsync));

    Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(GetGroupMemberInfoRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupMemberInfoResponse>(nameof(GetGroupMemberInfoAsync));

    Task SetAvatarAsync(SetAvatarRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetAvatarAsync));

    Task SetNicknameAsync(SetNicknameRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetNicknameAsync));

    Task SetBioAsync(SetBioRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetBioAsync));

    Task<GetCustomFaceUrlListResponse> GetCustomFaceUrlListAsync()
        => AdapterFeatureNotSupported.NotSupportedAsync<GetCustomFaceUrlListResponse>(nameof(GetCustomFaceUrlListAsync));

    Task<GetCookiesResponse> GetCookiesAsync(GetCookiesRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetCookiesResponse>(nameof(GetCookiesAsync));

    Task<GetCsrfTokenResponse> GetCsrfTokenAsync()
        => AdapterFeatureNotSupported.NotSupportedAsync<GetCsrfTokenResponse>(nameof(GetCsrfTokenAsync));
}
