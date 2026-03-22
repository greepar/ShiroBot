using Milky.Net.Client;
using MModel = Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class GroupService : IGroupService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task SetGroupNameAsync(SetGroupNameRequest request) =>
        await Milky.Group.SetGroupNameAsync(ModelMapper.Convert<MModel.SetGroupNameRequest>(request));

    public async Task SetGroupAvatarAsync(SetGroupAvatarRequest request) =>
        await Milky.Group.SetGroupAvatarAsync(ModelMapper.Convert<MModel.SetGroupAvatarRequest>(request));

    public async Task SetGroupMemberCardAsync(SetGroupMemberCardRequest request) =>
        await Milky.Group.SetGroupMemberCardAsync(ModelMapper.Convert<MModel.SetGroupMemberCardRequest>(request));

    public async Task SetGroupMemberSpecialTitleAsync(SetGroupMemberSpecialTitleRequest request) =>
        await Milky.Group.SetGroupMemberSpecialTitleAsync(ModelMapper.Convert<MModel.SetGroupMemberSpecialTitleRequest>(request));

    public async Task SetGroupMemberAdminAsync(SetGroupMemberAdminRequest request) =>
        await Milky.Group.SetGroupMemberAdminAsync(ModelMapper.Convert<MModel.SetGroupMemberAdminRequest>(request));

    public async Task SetGroupMemberMuteAsync(SetGroupMemberMuteRequest request) =>
        await Milky.Group.SetGroupMemberMuteAsync(ModelMapper.Convert<MModel.SetGroupMemberMuteRequest>(request));

    public async Task SetGroupWholeMuteAsync(SetGroupWholeMuteRequest request) =>
        await Milky.Group.SetGroupWholeMuteAsync(ModelMapper.Convert<MModel.SetGroupWholeMuteRequest>(request));

    public async Task KickGroupMemberAsync(KickGroupMemberRequest request) =>
        await Milky.Group.KickGroupMemberAsync(ModelMapper.Convert<MModel.KickGroupMemberRequest>(request));

    public async Task<GetGroupAnnouncementsResponse> GetGroupAnnouncementsAsync(GetGroupAnnouncementsRequest request) =>
        ModelMapper.Convert<GetGroupAnnouncementsResponse>(
            await Milky.Group.GetGroupAnnouncementsAsync(ModelMapper.Convert<MModel.GetGroupAnnouncementsRequest>(request)));

    public async Task SendGroupAnnouncementAsync(SendGroupAnnouncementRequest request) =>
        await Milky.Group.SendGroupAnnouncementAsync(ModelMapper.Convert<MModel.SendGroupAnnouncementRequest>(request));

    public async Task DeleteGroupAnnouncementAsync(DeleteGroupAnnouncementRequest request) =>
        await Milky.Group.DeleteGroupAnnouncementAsync(ModelMapper.Convert<MModel.DeleteGroupAnnouncementRequest>(request));

    public async Task<GetGroupEssenceMessagesResponse> GetGroupEssenceMessagesAsync(GetGroupEssenceMessagesRequest request) =>
        ModelMapper.Convert<GetGroupEssenceMessagesResponse>(
            await Milky.Group.GetGroupEssenceMessagesAsync(ModelMapper.Convert<MModel.GetGroupEssenceMessagesRequest>(request)));

    public async Task SetGroupEssenceMessageAsync(SetGroupEssenceMessageRequest request) =>
        await Milky.Group.SetGroupEssenceMessageAsync(ModelMapper.Convert<MModel.SetGroupEssenceMessageRequest>(request));

    public async Task QuitGroupAsync(QuitGroupRequest request) =>
        await Milky.Group.QuitGroupAsync(ModelMapper.Convert<MModel.QuitGroupRequest>(request));

    public async Task SendGroupMessageReactionAsync(SendGroupMessageReactionRequest request) =>
        await Milky.Group.SendGroupMessageReactionAsync(ModelMapper.Convert<MModel.SendGroupMessageReactionRequest>(request));

    public async Task SendGroupNudgeAsync(SendGroupNudgeRequest request) =>
        await Milky.Group.SendGroupNudgeAsync(ModelMapper.Convert<MModel.SendGroupNudgeRequest>(request));

    public async Task<GetGroupNotificationsResponse> GetGroupNotificationsAsync(GetGroupNotificationsRequest request) =>
        ModelMapper.Convert<GetGroupNotificationsResponse>(
            await Milky.Group.GetGroupNotificationsAsync(ModelMapper.Convert<MModel.GetGroupNotificationsRequest>(request)));

    public async Task AcceptGroupRequestAsync(AcceptGroupRequestRequest request) =>
        await Milky.Group.AcceptGroupRequestAsync(ModelMapper.Convert<MModel.AcceptGroupRequestRequest>(request));

    public async Task RejectGroupRequestAsync(RejectGroupRequestRequest request) =>
        await Milky.Group.RejectGroupRequestAsync(ModelMapper.Convert<MModel.RejectGroupRequestRequest>(request));

    public async Task AcceptGroupInvitationAsync(AcceptGroupInvitationRequest request) =>
        await Milky.Group.AcceptGroupInvitationAsync(ModelMapper.Convert<MModel.AcceptGroupInvitationRequest>(request));

    public async Task RejectGroupInvitationAsync(RejectGroupInvitationRequest request) =>
        await Milky.Group.RejectGroupInvitationAsync(ModelMapper.Convert<MModel.RejectGroupInvitationRequest>(request));
}
