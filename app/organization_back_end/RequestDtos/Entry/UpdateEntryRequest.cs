
namespace organization_back_end.RequestDtos.Entry;

public class UpdateEntryRequest : AddEntryRequest
{
    public required Guid EntryId { get; set; }
}