using Microsoft.AspNetCore.Mvc;
using organization_back_end.RequestDtos.Entry;

namespace organization_back_end.Services;

public class FileService
{
    private readonly BlobService _blobService;
    private readonly List<string> _allowedExtensions = new List<string> {".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx", ".pptx", ".txt"};

    public FileService(BlobService blobService)
    {
        _blobService = blobService;
    }
    
    public async Task<string> UploadFileAsync(IFormFile? file, FileRequest request, Guid id)
    {
        if (file is null || file.Length <= 0) return string.Empty;

        var extension = request.Extension;
        if (!_allowedExtensions.Contains(extension))
        {
            throw new Exception("File extension is not allowed");
        }

        return await _blobService.UploadFileAsync(file, $"{id}-{request.Name}{request.Extension}");
    }
    
    public async Task<FileStreamResult> DownloadFileAsync(string name)
    {
        return await _blobService.DownloadFileAsync(name);
    }
    
    public async Task DeleteFileAsync(string name)
    {
        await _blobService.DeleteFileAsync(name);
    }
}