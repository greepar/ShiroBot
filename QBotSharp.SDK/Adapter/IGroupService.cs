using QBotSharp.Model.Group.Requests;
using QBotSharp.Model.Group.Responses;

namespace QBotSharp.SDK.Adapter;

public interface IGroupService
{
    Task SetGroupNameAsync(SetGroupNameRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupNameAsync));

    Task SetGroupAvatarAsync(SetGroupAvatarRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupAvatarAsync));

    Task SetGroupMemberCardAsync(SetGroupMemberCardRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupMemberCardAsync));

    Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupMemberSpecialTitleAsync));

    Task SetGroupMemberAdminAsync(SetGroupMemberAdminRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupMemberAdminAsync));

    Task SetGroupMemberMuteAsync(SetGroupMemberMuteRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupMemberMuteAsync));

    Task SetGroupWholeMuteAsync(SetGroupWholeMuteRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupWholeMuteAsync));

    Task KickGroupMemberAsync(KickGroupMemberRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(KickGroupMemberAsync));

    Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(GetGroupAnnouncementsRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupAnnouncementsResponse>(nameof(GetGroupAnnouncementsAsync));

    Task SendGroupAnnouncementAsync(SendGroupAnnouncementRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SendGroupAnnouncementAsync));

    Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(DeleteGroupAnnouncementAsync));

    Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupEssenceMessagesResponse>(nameof(GetGroupEssenceMessagesAsync));

    Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SetGroupEssenceMessageAsync));

    Task QuitGroupAsync(QuitGroupRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(QuitGroupAsync));

    Task SendGroupMessageReactionAsync(SendGroupMessageReactionRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SendGroupMessageReactionAsync));

    Task SendGroupNudgeAsync(SendGroupNudgeRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(SendGroupNudgeAsync));

    Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(GetGroupNotificationsRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupNotificationsResponse>(nameof(GetGroupNotificationsAsync));

    Task AcceptGroupRequestAsync(AcceptGroupRequestRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(AcceptGroupRequestAsync));

    Task RejectGroupRequestAsync(RejectGroupRequestRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(RejectGroupRequestAsync));

    Task AcceptGroupInvitationAsync(AcceptGroupInvitationRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(AcceptGroupInvitationAsync));

    Task RejectGroupInvitationAsync(RejectGroupInvitationRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(RejectGroupInvitationAsync));
}
