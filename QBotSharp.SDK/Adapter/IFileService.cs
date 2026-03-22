using QBotSharp.Model.File.Requests;
using QBotSharp.Model.File.Responses;

namespace QBotSharp.SDK.Adapter;

public interface IFileService
{
    Task<UploadPrivateFileResponse> UploadPrivateFileAsync(UploadPrivateFileRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<UploadPrivateFileResponse>(nameof(UploadPrivateFileAsync));

    Task<UploadGroupFileResponse> UploadGroupFileAsync(UploadGroupFileRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<UploadGroupFileResponse>(nameof(UploadGroupFileAsync));

    Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetPrivateFileDownloadUrlResponse>(nameof(GetPrivateFileDownloadUrlAsync));

    Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupFileDownloadUrlResponse>(nameof(GetGroupFileDownloadUrlAsync));

    Task<GetGroupFilesResponse> GetGroupFilesAsync(GetGroupFilesRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<GetGroupFilesResponse>(nameof(GetGroupFilesAsync));

    Task MoveGroupFileAsync(MoveGroupFileRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(MoveGroupFileAsync));

    Task RenameGroupFileAsync(RenameGroupFileRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(RenameGroupFileAsync));

    Task DeleteGroupFileAsync(DeleteGroupFileRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(DeleteGroupFileAsync));

    Task<CreateGroupFolderResponse> CreateGroupFolderAsync(CreateGroupFolderRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync<CreateGroupFolderResponse>(nameof(CreateGroupFolderAsync));

    Task RenameGroupFolderAsync(RenameGroupFolderRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(RenameGroupFolderAsync));

    Task DeleteGroupFolderAsync(DeleteGroupFolderRequest request)
        => AdapterFeatureNotSupported.NotSupportedAsync(nameof(DeleteGroupFolderAsync));
}
