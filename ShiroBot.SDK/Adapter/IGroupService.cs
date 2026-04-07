using ShiroBot.Model.Group.Requests;
using ShiroBot.Model.Group.Responses;

namespace ShiroBot.SDK.Adapter;

public interface IGroupService
{
    Task SetGroupNameAsync(SetGroupNameRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupNameAsync)}'.");

    Task SetGroupNameAsync(long groupId, string newGroupName)
        => SetGroupNameAsync(new SetGroupNameRequest(groupId, newGroupName));

    Task SetGroupAvatarAsync(SetGroupAvatarRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupAvatarAsync)}'.");

    Task SetGroupAvatarAsync(long groupId, string imageUri)
        => SetGroupAvatarAsync(new SetGroupAvatarRequest(groupId, imageUri));

    Task SetGroupMemberCardAsync(SetGroupMemberCardRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberCardAsync)}'.");

    Task SetGroupMemberCardAsync(long groupId, long userId, string card)
        => SetGroupMemberCardAsync(new SetGroupMemberCardRequest(groupId, userId, card));

    Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberSpecialTitleAsync)}'.");

    Task SetGroupMemberSpecialTitleAsync(long groupId, long userId, string specialTitle)
        => SetGroupMemberSpecialTitleAsync(new SetGroupMemberSpecialTitleRequest(groupId, userId, specialTitle));

    Task SetGroupMemberAdminAsync(SetGroupMemberAdminRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberAdminAsync)}'.");

    Task SetGroupMemberAdminAsync(long groupId, long userId, bool isSet = true)
        => SetGroupMemberAdminAsync(new SetGroupMemberAdminRequest(groupId, userId, isSet));

    Task SetGroupMemberMuteAsync(SetGroupMemberMuteRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupMemberMuteAsync)}'.");

    Task SetGroupMemberMuteAsync(long groupId, long userId, int duration = 0)
        => SetGroupMemberMuteAsync(new SetGroupMemberMuteRequest(groupId, userId, duration));

    Task SetGroupWholeMuteAsync(SetGroupWholeMuteRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupWholeMuteAsync)}'.");

    Task SetGroupWholeMuteAsync(long groupId, bool isMute = true)
        => SetGroupWholeMuteAsync(new SetGroupWholeMuteRequest(groupId, isMute));

    Task KickGroupMemberAsync(KickGroupMemberRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(KickGroupMemberAsync)}'.");

    Task KickGroupMemberAsync(long groupId, long userId, bool rejectAddRequest = false)
        => KickGroupMemberAsync(new KickGroupMemberRequest(groupId, userId, rejectAddRequest));

    Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(GetGroupAnnouncementsRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupAnnouncementsAsync)}'.");

    Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(long groupId)
        => GetGroupAnnouncementsAsync(new GetGroupAnnouncementsRequest(groupId));

    Task SendGroupAnnouncementAsync(SendGroupAnnouncementRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupAnnouncementAsync)}'.");

    Task SendGroupAnnouncementAsync(long groupId, string content, string? imageUri = null)
        => SendGroupAnnouncementAsync(new SendGroupAnnouncementRequest(groupId, content, imageUri));

    Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(DeleteGroupAnnouncementAsync)}'.");

    Task DeleteGroupAnnouncementAsync(long groupId, string announcementId)
        => DeleteGroupAnnouncementAsync(new DeleteGroupAnnouncementRequest(groupId, announcementId));

    Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupEssenceMessagesAsync)}'.");

    Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(long groupId, int pageIndex, int pageSize)
        => GetGroupEssenceMessagesAsync(new GetGroupEssenceMessagesRequest(groupId, pageIndex, pageSize));

    Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SetGroupEssenceMessageAsync)}'.");

    Task SetGroupEssenceMessageAsync(long groupId, long messageSeq, bool isSet = true)
        => SetGroupEssenceMessageAsync(new SetGroupEssenceMessageRequest(groupId, messageSeq, isSet));

    Task QuitGroupAsync(QuitGroupRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(QuitGroupAsync)}'.");

    Task QuitGroupAsync(long groupId)
        => QuitGroupAsync(new QuitGroupRequest(groupId));

    Task SendGroupMessageReactionAsync(SendGroupMessageReactionRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupMessageReactionAsync)}'.");

    Task SendGroupMessageReactionAsync(long groupId, long messageSeq, string reaction, SendGroupMessageReactionRequestReactionType type = SendGroupMessageReactionRequestReactionType.Face ,bool isAdd = true)
        => SendGroupMessageReactionAsync(new SendGroupMessageReactionRequest(groupId, messageSeq,reaction,type,isAdd));

    Task SendGroupNudgeAsync(SendGroupNudgeRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(SendGroupNudgeAsync)}'.");

    Task SendGroupNudgeAsync(long groupId, long userId)
        => SendGroupNudgeAsync(new SendGroupNudgeRequest(groupId, userId));

    Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(GetGroupNotificationsRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupNotificationsAsync)}'.");

    Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(long? startNotificationSeq = null, bool isFiltered = false, int limit = 20)
        => GetGroupNotificationsAsync(new GetGroupNotificationsRequest(startNotificationSeq, isFiltered, limit));

    Task AcceptGroupRequestAsync(AcceptGroupRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(AcceptGroupRequestAsync)}'.");

    Task AcceptGroupRequestAsync(long notificationSeq, AcceptGroupRequestRequestNotificationType notificationType, long groupId, bool isFiltered = false)
        => AcceptGroupRequestAsync(new AcceptGroupRequestRequest(notificationSeq, notificationType, groupId, isFiltered));

    Task RejectGroupRequestAsync(RejectGroupRequestRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RejectGroupRequestAsync)}'.");

    Task RejectGroupRequestAsync(long notificationSeq, RejectGroupRequestRequestNotificationType notificationType, long groupId, bool isFiltered = false, string? reason = null)
        => RejectGroupRequestAsync(new RejectGroupRequestRequest(notificationSeq, notificationType, groupId, isFiltered, reason));

    Task AcceptGroupInvitationAsync(AcceptGroupInvitationRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(AcceptGroupInvitationAsync)}'.");

    Task AcceptGroupInvitationAsync(long groupId, long invitationSeq)
        => AcceptGroupInvitationAsync(new AcceptGroupInvitationRequest(groupId, invitationSeq));

    Task RejectGroupInvitationAsync(RejectGroupInvitationRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RejectGroupInvitationAsync)}'.");

    Task RejectGroupInvitationAsync(long groupId, long invitationSeq)
        => RejectGroupInvitationAsync(new RejectGroupInvitationRequest(groupId, invitationSeq));
}
