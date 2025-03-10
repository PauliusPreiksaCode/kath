using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace organization_back_end.Services;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration["Blob:Key"]);
        _containerClient = blobServiceClient.GetBlobContainerClient(configuration["Blob:ContainerName"]);
    }
    
    public async Task<string> UploadFileAsync(IFormFile file, string fullname)
    {
        var blobClient = _containerClient.GetBlobClient(fullname);
        
        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, true);
        return blobClient.Uri.ToString();
    }
    
    public async Task<FileStreamResult> DownloadFileAsync(string fullname)
    {
        var blobClient = _containerClient.GetBlobClient(fullname);
        
        if (!await blobClient.ExistsAsync())
        {
            throw new Exception("File not found");
        }

        var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        stream.Position = 0;
        
        var contentType = "application/octet-stream";
        var fileName = Path.GetFileName(fullname);

        var result = new FileStreamResult(stream, contentType)
        {
            FileDownloadName = fileName
        };
        
        return result;
    }
    
    public async Task DeleteFileAsync(string fullname)
    {
        var blobClient = _containerClient.GetBlobClient(fullname);
        
        if (!await blobClient.ExistsAsync())
        {
            return;
        }

        await blobClient.DeleteAsync();
    }
}