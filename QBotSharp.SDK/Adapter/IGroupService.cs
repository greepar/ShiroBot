using Milky.Net.Model;

namespace QBotSharp.SDK.Adapter;

public interface IGroupService
{
    Task SetGroupNameAsync(SetGroupNameRequest request);
    Task SetGroupAvatarAsync(SetGroupAvatarRequest request);
    Task SetGroupMemberCardAsync(SetGroupMemberCardRequest request);
    Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleRequest request);
    Task SetGroupMemberAdminAsync(SetGroupMemberAdminRequest request);
    Task SetGroupMemberMuteAsync(SetGroupMemberMuteRequest request);
    Task SetGroupWholeMuteAsync(SetGroupWholeMuteRequest request);
    Task KickGroupMemberAsync(KickGroupMemberRequest request);
    Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(GetGroupAnnouncementsRequest request);
    Task SendGroupAnnouncementAsync(SendGroupAnnouncementRequest request);
    Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementRequest request);
    Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesRequest request);
    Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageRequest request);
    Task QuitGroupAsync(QuitGroupRequest request);
    Task SendGroupMessageReactionAsync(SendGroupMessageReactionRequest request);
    Task SendGroupNudgeAsync(SendGroupNudgeRequest request);
    Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(GetGroupNotificationsRequest request);
    Task AcceptGroupRequestAsync(AcceptGroupRequestRequest request);
    Task RejectGroupRequestAsync(RejectGroupRequestRequest request);
    Task AcceptGroupInvitationAsync(AcceptGroupInvitationRequest request);
    Task RejectGroupInvitationAsync(RejectGroupInvitationRequest request);
}
