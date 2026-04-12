using ShiroBot.Model.Group.Requests;
using ShiroBot.Model.Group.Responses;
using ShiroBot.SDK.Adapter;

namespace ShiroBot.SDK.Plugin;

public interface IGroupContext : IGroupService
{
    Task SetGroupNameAsync(long groupId, string newGroupName) =>
        SetGroupNameAsync(new SetGroupNameRequest(groupId, newGroupName));

    Task SetGroupAvatarAsync(long groupId, string imageUri) =>
        SetGroupAvatarAsync(new SetGroupAvatarRequest(groupId, imageUri));

    Task SetGroupMemberCardAsync(long groupId, long userId, string card) =>
        SetGroupMemberCardAsync(new SetGroupMemberCardRequest(groupId, userId, card));

    Task SetGroupMemberSpecialTitleAsync(long groupId, long userId, string specialTitle) =>
        SetGroupMemberSpecialTitleAsync(new SetGroupMemberSpecialTitleRequest(groupId, userId, specialTitle));

    Task SetGroupMemberAdminAsync(long groupId, long userId, bool isSet = true) =>
        SetGroupMemberAdminAsync(new SetGroupMemberAdminRequest(groupId, userId, isSet));

    Task SetGroupMemberMuteAsync(long groupId, long userId, int duration = 0) =>
        SetGroupMemberMuteAsync(new SetGroupMemberMuteRequest(groupId, userId, duration));

    Task SetGroupWholeMuteAsync(long groupId, bool isMute = true) =>
        SetGroupWholeMuteAsync(new SetGroupWholeMuteRequest(groupId, isMute));

    Task KickGroupMemberAsync(long groupId, long userId, bool rejectAddRequest = false) =>
        KickGroupMemberAsync(new KickGroupMemberRequest(groupId, userId, rejectAddRequest));

    Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(long groupId) =>
        GetGroupAnnouncementsAsync(new GetGroupAnnouncementsRequest(groupId));

    Task SendGroupAnnouncementAsync(long groupId, string content, string? imageUri = null) =>
        SendGroupAnnouncementAsync(new SendGroupAnnouncementRequest(groupId, content, imageUri));

    Task DeleteGroupAnnouncementAsync(long groupId, string announcementId) =>
        DeleteGroupAnnouncementAsync(new DeleteGroupAnnouncementRequest(groupId, announcementId));

    Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(long groupId, int pageIndex, int pageSize) =>
        GetGroupEssenceMessagesAsync(new GetGroupEssenceMessagesRequest(groupId, pageIndex, pageSize));

    Task SetGroupEssenceMessageAsync(long groupId, long messageSeq, bool isSet = true) =>
        SetGroupEssenceMessageAsync(new SetGroupEssenceMessageRequest(groupId, messageSeq, isSet));

    Task QuitGroupAsync(long groupId) =>
        QuitGroupAsync(new QuitGroupRequest(groupId));

    Task SendGroupMessageReactionAsync(long groupId, long messageSeq, string reaction, SendGroupMessageReactionRequestReactionType type = SendGroupMessageReactionRequestReactionType.Face, bool isAdd = true) =>
        SendGroupMessageReactionAsync(new SendGroupMessageReactionRequest(groupId, messageSeq, reaction, type, isAdd));

    Task SendGroupNudgeAsync(long groupId, long userId) =>
        SendGroupNudgeAsync(new SendGroupNudgeRequest(groupId, userId));

    Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(long? startNotificationSeq = null, bool isFiltered = false, int limit = 20) =>
        GetGroupNotificationsAsync(new GetGroupNotificationsRequest(startNotificationSeq, isFiltered, limit));

    Task AcceptGroupRequestAsync(long notificationSeq, AcceptGroupRequestRequestNotificationType notificationType, long groupId, bool isFiltered = false) =>
        AcceptGroupRequestAsync(new AcceptGroupRequestRequest(notificationSeq, notificationType, groupId, isFiltered));

    Task RejectGroupRequestAsync(long notificationSeq, RejectGroupRequestRequestNotificationType notificationType, long groupId, bool isFiltered = false, string? reason = null) =>
        RejectGroupRequestAsync(new RejectGroupRequestRequest(notificationSeq, notificationType, groupId, isFiltered, reason));

    Task AcceptGroupInvitationAsync(long groupId, long invitationSeq) =>
        AcceptGroupInvitationAsync(new AcceptGroupInvitationRequest(groupId, invitationSeq));

    Task RejectGroupInvitationAsync(long groupId, long invitationSeq) =>
        RejectGroupInvitationAsync(new RejectGroupInvitationRequest(groupId, invitationSeq));
}
