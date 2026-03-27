using ShiroBot.Model.File.Requests;
using ShiroBot.Model.File.Responses;

namespace ShiroBot.SDK.Adapter;

public interface IFileService
{
    Task<UploadPrivateFileResponse> UploadPrivateFileAsync(UploadPrivateFileRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(UploadPrivateFileAsync)}'.");

    Task<UploadPrivateFileResponse> UploadPrivateFileAsync(long userId, string fileUri, string fileName)
        => UploadPrivateFileAsync(new UploadPrivateFileRequest(userId, fileUri, fileName));

    Task<UploadGroupFileResponse> UploadGroupFileAsync(UploadGroupFileRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(UploadGroupFileAsync)}'.");

    Task<UploadGroupFileResponse> UploadGroupFileAsync(long groupId, string fileUri, string fileName, string parentFolderId = "/")
        => UploadGroupFileAsync(new UploadGroupFileRequest(groupId, fileUri, fileName, parentFolderId));

    Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(GetPrivateFileDownloadUrlRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetPrivateFileDownloadUrlAsync)}'.");

    Task<GetPrivateFileDownloadUrlResponse> GetPrivateFileDownloadUrlAsync(long userId, string fileId, string fileHash)
        => GetPrivateFileDownloadUrlAsync(new GetPrivateFileDownloadUrlRequest(userId, fileId, fileHash));

    Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(GetGroupFileDownloadUrlRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupFileDownloadUrlAsync)}'.");

    Task<GetGroupFileDownloadUrlResponse> GetGroupFileDownloadUrlAsync(long groupId, string fileId)
        => GetGroupFileDownloadUrlAsync(new GetGroupFileDownloadUrlRequest(groupId, fileId));

    Task<GetGroupFilesResponse> GetGroupFilesAsync(GetGroupFilesRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(GetGroupFilesAsync)}'.");

    Task<GetGroupFilesResponse> GetGroupFilesAsync(long groupId, string parentFolderId = "/")
        => GetGroupFilesAsync(new GetGroupFilesRequest(groupId, parentFolderId));

    Task MoveGroupFileAsync(MoveGroupFileRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(MoveGroupFileAsync)}'.");

    Task MoveGroupFileAsync(long groupId, string fileId, string targetFolderId, string parentFolderId = "/")
        => MoveGroupFileAsync(new MoveGroupFileRequest(groupId, fileId, targetFolderId, parentFolderId));

    Task RenameGroupFileAsync(RenameGroupFileRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RenameGroupFileAsync)}'.");

    Task RenameGroupFileAsync(long groupId, string fileId, string newFileName, string parentFolderId = "/")
        => RenameGroupFileAsync(new RenameGroupFileRequest(groupId, fileId, newFileName, parentFolderId));

    Task DeleteGroupFileAsync(DeleteGroupFileRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(DeleteGroupFileAsync)}'.");

    Task DeleteGroupFileAsync(long groupId, string fileId)
        => DeleteGroupFileAsync(new DeleteGroupFileRequest(groupId, fileId));

    Task<CreateGroupFolderResponse> CreateGroupFolderAsync(CreateGroupFolderRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(CreateGroupFolderAsync)}'.");

    Task<CreateGroupFolderResponse> CreateGroupFolderAsync(long groupId, string folderName)
        => CreateGroupFolderAsync(new CreateGroupFolderRequest(groupId, folderName));

    Task RenameGroupFolderAsync(RenameGroupFolderRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(RenameGroupFolderAsync)}'.");

    Task RenameGroupFolderAsync(long groupId, string folderId, string newFolderName)
        => RenameGroupFolderAsync(new RenameGroupFolderRequest(groupId, folderId, newFolderName));

    Task DeleteGroupFolderAsync(DeleteGroupFolderRequest request)
        => throw new NotSupportedException($"Current adapter does not support '{nameof(DeleteGroupFolderAsync)}'.");

    Task DeleteGroupFolderAsync(long groupId, string folderId)
        => DeleteGroupFolderAsync(new DeleteGroupFolderRequest(groupId, folderId));
}
