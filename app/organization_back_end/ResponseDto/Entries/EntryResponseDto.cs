using organization_back_end.Entities;

namespace organization_back_end.ResponseDto.Entries;

public class EntryResponseDto : Entry
{
    public string FullName { get; set; }
}