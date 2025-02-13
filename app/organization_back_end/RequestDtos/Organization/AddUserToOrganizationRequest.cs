namespace organization_back_end.RequestDtos.Organization;

public class AddUserToOrganizationRequest
{
    public required string userId { get; set; }
    public required Guid organizationId { get; set; }
}