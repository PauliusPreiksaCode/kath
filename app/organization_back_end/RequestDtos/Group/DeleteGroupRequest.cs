namespace organization_back_end.RequestDtos.Group;

public class DeleteGroupRequest
{
    public Guid OrganizationId { get; set; }
    public Guid GroupId { get; set; }
}