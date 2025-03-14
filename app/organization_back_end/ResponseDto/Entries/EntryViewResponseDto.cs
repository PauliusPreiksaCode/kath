using File = organization_back_end.Entities.File;

namespace organization_back_end.ResponseDto.Entries;

public class EntryViewResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public required string Text { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModifyDate { get; set; }
    public Guid? FileId { get; set; }
    public File? File { get; set; }
    public string LicencedUserId { get; set; }
    public List<LinkedEntryResponseDto>? LinkedEntries { get; set; }
}

public class LinkedEntryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}