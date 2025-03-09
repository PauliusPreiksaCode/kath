using organization_back_end.Entities;

namespace organization_back_end.ResponseDto.Groups;

public class GroupResponseDto : Group
{
    public int EntriesCount { get; set; }
}