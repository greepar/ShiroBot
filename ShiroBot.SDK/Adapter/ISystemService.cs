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

    Task<GetUserProfileResponse> GetUserProfileAsync(long userId)
        => GetUserProfileAsync(new GetUserProfileRequest(userId));

    Task<GetFriendListResponse> GetFriendListAsync(GetFriendListRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetFriendListAsync)}'.");

    Task<GetFriendListResponse> GetFriendListAsync(bool noCache = false)
        => GetFriendListAsync(new GetFriendListRequest(noCache));

    Task<GetFriendInfoResponse> GetFriendInfoAsync(GetFriendInfoRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetFriendInfoAsync)}'.");

    Task<GetFriendInfoResponse> GetFriendInfoAsync(long userId, bool noCache = false)
        => GetFriendInfoAsync(new GetFriendInfoRequest(userId, noCache));

    Task<GetGroupListResponse> GetGroupListAsync(GetGroupListRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupListAsync)}'.");

    Task<GetGroupListResponse> GetGroupListAsync(bool noCache = false)
        => GetGroupListAsync(new GetGroupListRequest(noCache));

    Task<GetGroupInfoResponse> GetGroupInfoAsync(GetGroupInfoRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupInfoAsync)}'.");

    Task<GetGroupInfoResponse> GetGroupInfoAsync(long groupId, bool noCache = false)
        => GetGroupInfoAsync(new GetGroupInfoRequest(groupId, noCache));

    Task<GetGroupMemberListResponse> GetGroupMemberListAsync(GetGroupMemberListRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupMemberListAsync)}'.");

    Task<GetGroupMemberListResponse> GetGroupMemberListAsync(long groupId, bool noCache = false)
        => GetGroupMemberListAsync(new GetGroupMemberListRequest(groupId, noCache));

    Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(GetGroupMemberInfoRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupMemberInfoAsync)}'.");

    Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(long groupId, long userId, bool noCache = false)
        => GetGroupMemberInfoAsync(new GetGroupMemberInfoRequest(groupId, userId, noCache));

    Task SetAvatarAsync(SetAvatarRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetAvatarAsync)}'.");

    Task SetAvatarAsync(string uri)
        => SetAvatarAsync(new SetAvatarRequest(uri));

    Task SetNicknameAsync(SetNicknameRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetNicknameAsync)}'.");

    Task SetNicknameAsync(string newNickname)
        => SetNicknameAsync(new SetNicknameRequest(newNickname));

    Task SetBioAsync(SetBioRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetBioAsync)}'.");

    Task SetBioAsync(string newBio)
        => SetBioAsync(new SetBioRequest(newBio));

    Task<GetCustomFaceUrlListResponse> GetCustomFaceUrlListAsync()
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetCustomFaceUrlListAsync)}'.");

    Task<GetCookiesResponse> GetCookiesAsync(GetCookiesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetCookiesAsync)}'.");

    Task<GetCookiesResponse> GetCookiesAsync(string domain)
        => GetCookiesAsync(new GetCookiesRequest(domain));

    Task<GetCsrfTokenResponse> GetCsrfTokenAsync()
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetCsrfTokenAsync)}'.");
}
