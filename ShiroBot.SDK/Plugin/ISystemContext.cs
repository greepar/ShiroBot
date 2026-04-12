using ShiroBot.Model.System.Requests;
using ShiroBot.Model.System.Responses;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.SDK.Plugin;

public interface ISystemContext : ISystemService
{
    Task<GetUserProfileResponse> GetUserProfileAsync(long userId) =>
        GetUserProfileAsync(new GetUserProfileRequest(userId));

    Task<GetFriendListResponse> GetFriendListAsync(bool noCache = false) =>
        GetFriendListAsync(new GetFriendListRequest(noCache));

    Task<GetFriendInfoResponse> GetFriendInfoAsync(long userId, bool noCache = false) =>
        GetFriendInfoAsync(new GetFriendInfoRequest(userId, noCache));

    Task<GetGroupListResponse> GetGroupListAsync(bool noCache = false) =>
        GetGroupListAsync(new GetGroupListRequest(noCache));

    Task<GetGroupInfoResponse> GetGroupInfoAsync(long groupId, bool noCache = false) =>
        GetGroupInfoAsync(new GetGroupInfoRequest(groupId, noCache));

    Task<GetGroupMemberListResponse> GetGroupMemberListAsync(long groupId, bool noCache = false) =>
        GetGroupMemberListAsync(new GetGroupMemberListRequest(groupId, noCache));

    Task<GetGroupMemberInfoResponse> GetGroupMemberInfoAsync(long groupId, long userId, bool noCache = false) =>
        GetGroupMemberInfoAsync(new GetGroupMemberInfoRequest(groupId, userId, noCache));

    Task SetAvatarAsync(string uri) =>
        SetAvatarAsync(new SetAvatarRequest(uri));

    Task SetNicknameAsync(string newNickname) =>
        SetNicknameAsync(new SetNicknameRequest(newNickname));

    Task SetBioAsync(string newBio) =>
        SetBioAsync(new SetBioRequest(newBio));

    Task<GetCookiesResponse> GetCookiesAsync(string domain) =>
        GetCookiesAsync(new GetCookiesRequest(domain));
}
