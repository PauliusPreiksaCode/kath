namespace organization_back_end.RequestDtos.Entry;

public class FindReferencesRequest
{
    public required string Text { get; set; }
    public required Guid GroupId { get; set; }
    public Guid? EntryId { get; set; }
}