using Microsoft.AspNetCore.Mvc;
using organization_back_end.RequestDtos.Entry;

namespace organization_back_end.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile? file, FileRequest request, Guid id);
    Task<FileStreamResult> DownloadFileAsync(string name);
    Task DeleteFileAsync(string name);
}