using organization_back_end.Entities;
using organization_back_end.RequestDtos.Organization;
using organization_back_end.ResponseDto.Organizations;
using organization_back_end.ResponseDto.Users;

namespace organization_back_end.Interfaces;

public interface IOrganizationService
{
    Task<ICollection<OrganizationResponseDto>> GetOrganizations(string userId);
    Task<bool> IsUserOrganizationOwner(string userId, Guid organizationId);
    Task CreateOrganization(string userId, CreateOrganizationRequest request, OrganizationOwner user);
    Task AddUserToOrganization(string userId, Guid organizationId);
    Task RemoveUserFromOrganization(string userId, Guid organizationId);
    Task UpdateOrganization(Guid organizationId, string name, string description);
    Task DeleteOrganization(Guid organizationId);
    Task<ICollection<UserResponseDto>> GetOrganizationUsers(Guid organizationId, string userId);
    Task<ICollection<UserResponseDto>> GetNonOrganizationUsers(Guid organizationId, string userId);
}