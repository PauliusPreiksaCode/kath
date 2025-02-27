using organization_back_end.Entities;

namespace organization_back_end.ResponseDto.Organizations;

public class OrganizationResponseDto : Organization
{
    public int MembersCount { get; set; }
}