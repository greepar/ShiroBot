using ShiroBot.SDK.Adapter;

namespace ShiroBot.DemoAdapter.AdapterImpl;

public class FileService : IFileService
{
    public Task<UploadPrivateFileResponse> UploadPrivateFileAsync(UploadPrivateFileRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<UploadGroupFileResponse> UploadGroupFileAsync(UploadGroupFileRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<GetGroupFilesResponse> GetGroupFilesAsync(GetGroupFilesRequest request)
    {
        throw new NotImplementedException();
    }

    public Task MoveGroupFileAsync(MoveGroupFileRequest request)
    {
        throw new NotImplementedException();
    }

    public Task RenameGroupFileAsync(RenameGroupFileRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGroupFileAsync(DeleteGroupFileRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<CreateGroupFolderResponse> CreateGroupFolderAsync(CreateGroupFolderRequest request)
    {
        throw new NotImplementedException();
    }

    public Task RenameGroupFolderAsync(RenameGroupFolderRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteGroupFolderAsync(DeleteGroupFolderRequest request)
    {
        throw new NotImplementedException();
    }
}
