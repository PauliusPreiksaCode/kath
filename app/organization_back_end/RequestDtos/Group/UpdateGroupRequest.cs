namespace organization_back_end.RequestDtos.Group;

public class UpdateGroupRequest
{
    public Guid OrganizationId { get; set; }
    public Guid GroupId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}