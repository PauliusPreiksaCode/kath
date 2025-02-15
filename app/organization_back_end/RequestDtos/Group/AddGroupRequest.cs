namespace organization_back_end.RequestDtos.Group;

public class AddGroupRequest
{
    public Guid OrganizationId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}