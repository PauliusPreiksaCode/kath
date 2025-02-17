namespace organization_back_end.RequestDtos.Entry;

public class DeleteEntryRequest
{
    public Guid EntryId { get; set; }
    public Guid GroupId { get; set; }
}