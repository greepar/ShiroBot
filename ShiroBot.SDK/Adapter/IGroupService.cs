using ShiroBot.Model.Group.Requests;
using ShiroBot.Model.Group.Responses;

namespace ShiroBot.SDK.Adapter;

public interface IGroupService
{
    Task SetGroupNameAsync(SetGroupNameRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupNameAsync)}'.");

    Task SetGroupAvatarAsync(SetGroupAvatarRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupAvatarAsync)}'.");

    Task SetGroupMemberCardAsync(SetGroupMemberCardRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberCardAsync)}'.");

    Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberSpecialTitleAsync)}'.");

    Task SetGroupMemberAdminAsync(SetGroupMemberAdminRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberAdminAsync)}'.");

    Task SetGroupMemberMuteAsync(SetGroupMemberMuteRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberMuteAsync)}'.");

    Task SetGroupWholeMuteAsync(SetGroupWholeMuteRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupWholeMuteAsync)}'.");

    Task KickGroupMemberAsync(KickGroupMemberRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(KickGroupMemberAsync)}'.");

    Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(GetGroupAnnouncementsRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupAnnouncementsAsync)}'.");

    Task SendGroupAnnouncementAsync(SendGroupAnnouncementRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupAnnouncementAsync)}'.");

    Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(DeleteGroupAnnouncementAsync)}'.");

    Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupEssenceMessagesAsync)}'.");

    Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupEssenceMessageAsync)}'.");

    Task QuitGroupAsync(QuitGroupRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(QuitGroupAsync)}'.");

    Task SendGroupMessageReactionAsync(SendGroupMessageReactionRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupMessageReactionAsync)}'.");

    Task SendGroupNudgeAsync(SendGroupNudgeRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupNudgeAsync)}'.");

    Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(GetGroupNotificationsRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupNotificationsAsync)}'.");

    Task AcceptGroupRequestAsync(AcceptGroupRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(AcceptGroupRequestAsync)}'.");

    Task RejectGroupRequestAsync(RejectGroupRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RejectGroupRequestAsync)}'.");

    Task AcceptGroupInvitationAsync(AcceptGroupInvitationRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(AcceptGroupInvitationAsync)}'.");

    Task RejectGroupInvitationAsync(RejectGroupInvitationRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RejectGroupInvitationAsync)}'.");
}
