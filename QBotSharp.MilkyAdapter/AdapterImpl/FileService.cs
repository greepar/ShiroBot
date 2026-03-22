using Milky.Net.Client;
using MModel = Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class FileService : IFileService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;

    public async Task<UploadPrivateFileResponse> UploadPrivateFileAsync(UploadPrivateFileRequest request) =>
        ModelMapper.Convert<UploadPrivateFileResponse>(
            await Milky.File.UploadPrivateFileAsync(ModelMapper.Convert<MModel.UploadPrivateFileRequest>(request)));

    public async Task<UploadGroupFileResponse> UploadGroupFileAsync(UploadGroupFileRequest request) =>
        ModelMapper.Convert<UploadGroupFileResponse>(
            await Milky.File.UploadGroupFileAsync(ModelMapper.Convert<MModel.UploadGroupFileRequest>(request)));

    public async Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlRequest request) =>
        ModelMapper.Convert<GetPrivateFileDownloadUrlResponse>(
            await Milky.File.GetPrivateFileDownloadUrlAsync(ModelMapper.Convert<MModel.GetPrivateFileDownloadUrlRequest>(request)));

    public async Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlRequest request) =>
        ModelMapper.Convert<GetGroupFileDownloadUrlResponse>(
            await Milky.File.GetGroupFileDownloadUrlAsync(ModelMapper.Convert<MModel.GetGroupFileDownloadUrlRequest>(request)));

    public async Task<GetGroupFilesResponse> GetGroupFilesAsync(GetGroupFilesRequest request) =>
        ModelMapper.Convert<GetGroupFilesResponse>(
            await Milky.File.GetGroupFilesAsync(ModelMapper.Convert<MModel.GetGroupFilesRequest>(request)));

    public async Task MoveGroupFileAsync(MoveGroupFileRequest request) =>
        await Milky.File.MoveGroupFileAsync(ModelMapper.Convert<MModel.MoveGroupFileRequest>(request));

    public async Task RenameGroupFileAsync(RenameGroupFileRequest request) =>
        await Milky.File.RenameGroupFileAsync(ModelMapper.Convert<MModel.RenameGroupFileRequest>(request));

    public async Task DeleteGroupFileAsync(DeleteGroupFileRequest request) =>
        await Milky.File.DeleteGroupFileAsync(ModelMapper.Convert<MModel.DeleteGroupFileRequest>(request));

    public async Task<CreateGroupFolderResponse> CreateGroupFolderAsync(CreateGroupFolderRequest request) =>
        ModelMapper.Convert<CreateGroupFolderResponse>(
            await Milky.File.CreateGroupFolderAsync(ModelMapper.Convert<MModel.CreateGroupFolderRequest>(request)));

    public async Task RenameGroupFolderAsync(RenameGroupFolderRequest request) =>
        await Milky.File.RenameGroupFolderAsync(ModelMapper.Convert<MModel.RenameGroupFolderRequest>(request));

    public async Task DeleteGroupFolderAsync(DeleteGroupFolderRequest request) =>
        await Milky.File.DeleteGroupFolderAsync(ModelMapper.Convert<MModel.DeleteGroupFolderRequest>(request));
}
