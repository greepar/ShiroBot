using QBotSharp.Core;
using QBotSharp.Model.Group.Requests;
using QBotSharp.Model.Group.Responses;
using QBotSharp.SDK.Adapter;
using QBotSharp.SDK.Plugin;

namespace QBotSharp.Hosting.Context;

public class GroupContext(IGroupService group) : IGroupContext
{
    public Task SetGroupNameAsync(SetGroupNameRequest request) =>
        group.SetGroupNameAsync(request);

    public Task SetGroupAvatarAsync(SetGroupAvatarRequest request) =>
        group.SetGroupAvatarAsync(request);

    public Task SetGroupMemberCardAsync(SetGroupMemberCardRequest request) =>
        group.SetGroupMemberCardAsync(request);

    public Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleRequest request) =>
        group.SetGroupMemberSpecialTitleAsync(request);

    public Task SetGroupMemberAdminAsync(SetGroupMemberAdminRequest request) =>
        group.SetGroupMemberAdminAsync(request);

    public Task SetGroupMemberMuteAsync(SetGroupMemberMuteRequest request) =>
        group.SetGroupMemberMuteAsync(request);

    public Task SetGroupWholeMuteAsync(SetGroupWholeMuteRequest request) =>
        group.SetGroupWholeMuteAsync(request);

    public Task KickGroupMemberAsync(KickGroupMemberRequest request) =>
        group.KickGroupMemberAsync(request);

    public Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(GetGroupAnnouncementsRequest request) =>
        group.GetGroupAnnouncementsAsync(request);

    public Task SendGroupAnnouncementAsync(SendGroupAnnouncementRequest request) =>
        group.SendGroupAnnouncementAsync(request);

    public Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementRequest request) =>
        group.DeleteGroupAnnouncementAsync(request);

    public Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesRequest request) =>
        group.GetGroupEssenceMessagesAsync(request);

    public Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageRequest request) =>
        group.SetGroupEssenceMessageAsync(request);

    public Task QuitGroupAsync(QuitGroupRequest request) =>
        group.QuitGroupAsync(request);

    public Task SendGroupMessageReactionAsync(SendGroupMessageReactionRequest request)
    {
        ConsoleHelper.Info($"[Plugin -> Group] Sending reaction to message in {request.GroupId}");
        return group.SendGroupMessageReactionAsync(request);
    }

    public Task SendGroupNudgeAsync(SendGroupNudgeRequest request) =>
        group.SendGroupNudgeAsync(request);

    public Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(GetGroupNotificationsRequest request) =>
        group.GetGroupNotificationsAsync(request);

    public Task AcceptGroupRequestAsync(AcceptGroupRequestRequest request) =>
        group.AcceptGroupRequestAsync(request);

    public Task RejectGroupRequestAsync(RejectGroupRequestRequest request) =>
        group.RejectGroupRequestAsync(request);

    public Task AcceptGroupInvitationAsync(AcceptGroupInvitationRequest request) =>
        group.AcceptGroupInvitationAsync(request);

    public Task RejectGroupInvitationAsync(RejectGroupInvitationRequest request) =>
        group.RejectGroupInvitationAsync(request);
}
