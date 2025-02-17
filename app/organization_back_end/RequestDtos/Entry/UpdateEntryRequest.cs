
namespace organization_back_end.RequestDtos.Entry;

public class UpdateEntryRequest : AddEntryRequest
{
    public Guid EntryId { get; set; }
}