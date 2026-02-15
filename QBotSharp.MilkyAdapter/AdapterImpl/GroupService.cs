using Milky.Net.Client;
using Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class GroupService : IGroupService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task SetGroupNameAsync(SetGroupNameRequest request)
    {
        await Milky.Group.SetGroupNameAsync(request);
    }

    public async Task SetGroupAvatarAsync(SetGroupAvatarRequest request)
    {
        await Milky.Group.SetGroupAvatarAsync(request);
    }

    public async Task SetGroupMemberCardAsync(SetGroupMemberCardRequest request)
    {
        await Milky.Group.SetGroupMemberCardAsync(request);
    }

    public async Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleRequest request)
    {
        await Milky.Group.SetGroupMemberSpecialTitleAsync(request);
    }

    public async Task SetGroupMemberAdminAsync(SetGroupMemberAdminRequest request)
    {
        await Milky.Group.SetGroupMemberAdminAsync(request);
    }

    public async Task SetGroupMemberMuteAsync(SetGroupMemberMuteRequest request)
    {
        await Milky.Group.SetGroupMemberMuteAsync(request);
    }

    public async Task SetGroupWholeMuteAsync(SetGroupWholeMuteRequest request)
    {
        await Milky.Group.SetGroupWholeMuteAsync(request);
    }

    public async Task KickGroupMemberAsync(KickGroupMemberRequest request)
    {
        await Milky.Group.KickGroupMemberAsync(request);
    }

    public async Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(GetGroupAnnouncementsRequest request)
    {
        return await Milky.Group.GetGroupAnnouncementsAsync(request);
    }

    public async Task SendGroupAnnouncementAsync(SendGroupAnnouncementRequest request)
    {
        await Milky.Group.SendGroupAnnouncementAsync(request);
    }

    public async Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementRequest request)
    {
        await Milky.Group.DeleteGroupAnnouncementAsync(request);
    }

    public async Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesRequest request)
    {
        return await Milky.Group.GetGroupEssenceMessagesAsync(request);
    }

    public async Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageRequest request)
    {
        await Milky.Group.SetGroupEssenceMessageAsync(request);
    }

    public async Task QuitGroupAsync(QuitGroupRequest request)
    {
        await Milky.Group.QuitGroupAsync(request);
    }

    public async Task SendGroupMessageReactionAsync(SendGroupMessageReactionRequest request)
    {
        await Milky.Group.SendGroupMessageReactionAsync(request);
    }

    public async Task SendGroupNudgeAsync(SendGroupNudgeRequest request)
    {
        await Milky.Group.SendGroupNudgeAsync(request);
    }

    public async Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(GetGroupNotificationsRequest request)
    {
        return await Milky.Group.GetGroupNotificationsAsync(request);
    }

    public async Task AcceptGroupRequestAsync(AcceptGroupRequestRequest request)
    {
        await Milky.Group.AcceptGroupRequestAsync(request);
    }

    public async Task RejectGroupRequestAsync(RejectGroupRequestRequest request)
    {
        await Milky.Group.RejectGroupRequestAsync(request);
    }

    public async Task AcceptGroupInvitationAsync(AcceptGroupInvitationRequest request)
    {
        await Milky.Group.AcceptGroupInvitationAsync(request);
    }

    public async Task RejectGroupInvitationAsync(RejectGroupInvitationRequest request)
    {
        await Milky.Group.RejectGroupInvitationAsync(request);
    }
}