using Microsoft.AspNetCore.Mvc;

namespace organization_back_end.Interfaces;

public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file, string fullname);
    Task<FileStreamResult> DownloadFileAsync(string fullname);
    Task DeleteFileAsync(string fullname);
}