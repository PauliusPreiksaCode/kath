namespace organization_back_end.RequestDtos.Organization;

public class CreateOrganizationRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}