using ShiroBot.Model.System.Requests;
using ShiroBot.Model.System.Responses;

namespace ShiroBot.SDK.Adapter;

public interface ISystemService
{
    Task<GetLoginInfoResponse> GetLoginInfoAsync()
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetLoginInfoAsync)}'.");

    Task<GetImplInfoResponse> GetImplInfoAsync()
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetImplInfoAsync)}'.");

    Task<GetUserProfileResponse> GetUserProfileAsync(GetUserProfileRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetUserProfileAsync)}'.");

    Task<GetFriendListResponse> GetFriendListAsync(GetFriendListRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetFriendListAsync)}'.");

    Task<GetFriendInfoResponse> GetFriendInfoAsync(GetFriendInfoRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetFriendInfoAsync)}'.");

    Task<GetGroupListResponse> GetGroupListAsync(GetGroupListRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupListAsync)}'.");

    Task<GetGroupInfoResponse> GetGroupInfoAsync(GetGroupInfoRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupInfoAsync)}'.");

    Task<GetGroupMemberListResponse> GetGroupMemberListAsync(GetGroupMemberListRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupMemberListAsync)}'.");

    Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(GetGroupMemberInfoRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupMemberInfoAsync)}'.");

    Task SetAvatarAsync(SetAvatarRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetAvatarAsync)}'.");

    Task SetNicknameAsync(SetNicknameRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetNicknameAsync)}'.");

    Task SetBioAsync(SetBioRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetBioAsync)}'.");

    Task<GetCustomFaceUrlListResponse> GetCustomFaceUrlListAsync()
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetCustomFaceUrlListAsync)}'.");

    Task<GetCookiesResponse> GetCookiesAsync(GetCookiesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetCookiesAsync)}'.");

    Task<GetCsrfTokenResponse> GetCsrfTokenAsync()
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetCsrfTokenAsync)}'.");
}
