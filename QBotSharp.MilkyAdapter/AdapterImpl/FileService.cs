using Milky.Net.Client;
using Milky.Net.Model;
using QBotSharp.MilkyAdapter.Milky;
using QBotSharp.SDK.Adapter;

namespace QBotSharp.MilkyAdapter.AdapterImpl;

public class FileService :IFileService
{
    private static MilkyClient Milky => MilkyClientManager.Instance;
    
    public async Task<UploadPrivateFileResponse> UploadPrivateFileAsync(UploadPrivateFileRequest request)
    {
        return await Milky.File.UploadPrivateFileAsync(request);
    }

    public async Task<UploadGroupFileResponse> UploadGroupFileAsync(UploadGroupFileRequest request)
    {
        return await Milky.File.UploadGroupFileAsync(request);
    }

    public async Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlRequest request)
    {
        return await Milky.File.GetPrivateFileDownloadUrlAsync(request);
    }

    public async Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlRequest request)
    {
        return await Milky.File.GetGroupFileDownloadUrlAsync(request);
    }

    public async Task<GetGroupFilesResponse> GetGroupFilesAsync(GetGroupFilesRequest request)
    {
        return await Milky.File.GetGroupFilesAsync(request);
    }

    public async Task MoveGroupFileAsync(MoveGroupFileRequest request)
    {
        await Milky.File.MoveGroupFileAsync(request);
    }

    public async Task RenameGroupFileAsync(RenameGroupFileRequest request)
    {
        await Milky.File.RenameGroupFileAsync(request);
    }

    public async Task DeleteGroupFileAsync(DeleteGroupFileRequest request)
    {
        await Milky.File.DeleteGroupFileAsync(request);
    }

    public async Task<CreateGroupFolderResponse> CreateGroupFolderAsync(CreateGroupFolderRequest request)
    {
        return await Milky.File.CreateGroupFolderAsync(request);
    }

    public async Task RenameGroupFolderAsync(RenameGroupFolderRequest request)
    {
        await Milky.File.RenameGroupFolderAsync(request);
    }

    public async Task DeleteGroupFolderAsync(DeleteGroupFolderRequest request)
    {
        await Milky.File.DeleteGroupFolderAsync(request);
    }
}