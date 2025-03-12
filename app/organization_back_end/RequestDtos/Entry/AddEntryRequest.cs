namespace organization_back_end.RequestDtos.Entry;

public class AddEntryRequest : FileRequest
{
    public required string EntryName { get; set; }
    public required string Text { get; set; }
    public required Guid GroupId { get; set; }
    public required Guid OrganizationId { get; set; }
    
    public List<Guid>? LinkedEntries { get; set; }
}