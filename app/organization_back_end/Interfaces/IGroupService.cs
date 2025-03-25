using organization_back_end.RequestDtos.Group;
using organization_back_end.ResponseDto.Groups;

namespace organization_back_end.Interfaces;

public interface IGroupService
{
    Task CreateGroup(AddGroupRequest request);
    Task<ICollection<GroupResponseDto>> GetGroups(Guid organizationId);
    Task UpdateGroup(UpdateGroupRequest request);
    Task DeleteGroup(DeleteGroupRequest request);
    Task DeleteGroup(Guid groupId, Guid organizationId);
}