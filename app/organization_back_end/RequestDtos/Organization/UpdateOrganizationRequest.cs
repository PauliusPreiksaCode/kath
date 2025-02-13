namespace organization_back_end.RequestDtos.Organization;

public class UpdateOrganizationRequest
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}