using Milky.Net.Model;

namespace QBotSharp.SDK.Adapter;

public interface IFileService
{
    Task<UploadPrivateFileResponse> UploadPrivateFileAsync(UploadPrivateFileRequest request);
    Task<UploadGroupFileResponse> UploadGroupFileAsync(UploadGroupFileRequest request);
    Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlRequest request);
    Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlRequest request);
    Task<GetGroupFilesResponse> GetGroupFilesAsync(GetGroupFilesRequest request);
    Task MoveGroupFileAsync(MoveGroupFileRequest request);
    Task RenameGroupFileAsync(RenameGroupFileRequest request);
    Task DeleteGroupFileAsync(DeleteGroupFileRequest request);
    Task<CreateGroupFolderResponse> CreateGroupFolderAsync(CreateGroupFolderRequest request);
    Task RenameGroupFolderAsync(RenameGroupFolderRequest request);
    Task DeleteGroupFolderAsync(DeleteGroupFolderRequest request);
}
