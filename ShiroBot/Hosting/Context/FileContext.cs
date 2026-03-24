using ShiroBot.Model.File.Requests;
using ShiroBot.Model.File.Responses;
using ShiroBot.Core;
using ShiroBot.SDK.Adapter;
using ShiroBot.SDK.Plugin;

namespace ShiroBot.Hosting.Context;

public class FileContext(IFileService file) : IFileContext
{
    public Task<UploadPrivateFileResponse> UploadPrivateFileAsync(UploadPrivateFileRequest request)
    {
        ConsoleHelper.Info($"[Plugin -> File] Uploading private file: {request.FileName}");
        return file.UploadPrivateFileAsync(request);
    }

    public Task<UploadGroupFileResponse> UploadGroupFileAsync(UploadGroupFileRequest request)
    {
        ConsoleHelper.Info($"[Plugin -> File] Uploading group file to {request.GroupId}: {request.FileName}");
        return file.UploadGroupFileAsync(request);
    }

    public Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlRequest request) =>
        file.GetPrivateFileDownloadUrlAsync(request);

    public Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlRequest request) =>
        file.GetGroupFileDownloadUrlAsync(request);

    public Task<GetGroupFilesResponse> GetGroupFilesAsync(GetGroupFilesRequest request) =>
        file.GetGroupFilesAsync(request);

    public Task MoveGroupFileAsync(MoveGroupFileRequest request) =>
        file.MoveGroupFileAsync(request);

    public Task RenameGroupFileAsync(RenameGroupFileRequest request) =>
        file.RenameGroupFileAsync(request);

    public Task DeleteGroupFileAsync(DeleteGroupFileRequest request) =>
        file.DeleteGroupFileAsync(request);

    public Task<CreateGroupFolderResponse> CreateGroupFolderAsync(CreateGroupFolderRequest request) =>
        file.CreateGroupFolderAsync(request);

    public Task RenameGroupFolderAsync(RenameGroupFolderRequest request) =>
        file.RenameGroupFolderAsync(request);

    public Task DeleteGroupFolderAsync(DeleteGroupFolderRequest request) =>
        file.DeleteGroupFolderAsync(request);
}
