namespace organization_back_end.RequestDtos.Entry;

public class FileRequest
{
    public IFormFile? File { get; set; }
    public string Name { get; set; }
    public string Extension { get; set; }
}