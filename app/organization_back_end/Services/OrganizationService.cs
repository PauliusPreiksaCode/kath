using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.RequestDtos.Organization;

namespace organization_back_end.Services;

public class OrganizationService
{
    private readonly SystemContext _systemContext;
    private readonly UserManager<User> _userManager;
    private readonly GroupService _groupService;

    public OrganizationService(SystemContext systemContext, UserManager<User> userManager, GroupService groupService)
    {
        _systemContext = systemContext;
        _userManager = userManager;
        _groupService = groupService;
    }
    
    public async Task<ICollection<Organization>> GetOrganizations(string userId)
    {
        var userOrganizationsId = await _systemContext.OrganizationUsers
            .Where(u => u.UserId.Equals(userId))
            .Select(u => u.OrganizationId)
            .Distinct()
            .ToListAsync();

        var organizations = await _systemContext.Organizations
            .AsNoTracking()
            .Include(o => o.Groups)
            .Where(o => userOrganizationsId.Contains(o.Id))
            .ToListAsync();
        
        return organizations;
    }
    
    public async Task<bool> IsUserOrganizationOwner(string userId, Guid organizationId)
    {
        var organization = await _systemContext.Organizations
            .FirstOrDefaultAsync(x => x.Id.Equals(organizationId));
        
        return organization?.OwnerId.Equals(userId) ?? false;
    }
    
    private async Task<bool> IsUserOrganizationMember(string userId, Guid organizationId)
    {
        var organizationUser = await _systemContext.OrganizationUsers
            .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.OrganizationId.Equals(organizationId));
        
        return organizationUser is not null;
    }

    public async Task CreateOrganization(string userId, CreateOrganizationRequest request, OrganizationOwner user)
    {
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreationDate = DateTime.Now,
            Users = new List<OrganizationUser>(),
            OwnerId = userId,
            Owner = user
        };
        await _systemContext.Organizations.AddAsync(organization);
        
        var organizationUser = new OrganizationUser
        {
            Id = Guid.NewGuid(),
            OrganizationId = organization.Id,
            UserId = userId,
            Organization = organization,
            User = user
        };        
        await _systemContext.OrganizationUsers.AddAsync(organizationUser);
        
        organization.Users.Add(organizationUser);
        
        user.Organizations ??= new List<Organization>();
        user.Organizations.Add(organization);
        user.OrganizationUsers ??= new List<OrganizationUser>();
        user.OrganizationUsers.Add(organizationUser);
        
        await _systemContext.SaveChangesAsync();
    }
    
    public async Task AddUserToOrganization(string userId, Guid organizationId)
    {
        if(await IsUserOrganizationMember(userId, organizationId))
            return;
        
        var organization = await _systemContext.Organizations
            .Include(x=> x.Users)
            .FirstOrDefaultAsync(x => x.Id.Equals(organizationId));
        
        var user = await _userManager.FindByIdAsync(userId);

        if (user is not null && organization is not null)
        {
            var organizationUser = new OrganizationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = organization.Id,
                UserId = userId,
                Organization = organization,
                User = (LicencedUser)user
            };
            await _systemContext.OrganizationUsers.AddAsync(organizationUser);

            organization.Users.Add(organizationUser);

            (user as LicencedUser)!.OrganizationUsers ??= new List<OrganizationUser>();
            (user as LicencedUser)!.OrganizationUsers?.Add(organizationUser);

            await _systemContext.SaveChangesAsync();
        }
    }
    
    public async Task RemoveUserFromOrganization(string userId, Guid organizationId)
    {
        if(!await IsUserOrganizationMember(userId, organizationId))
            return;
        
        var organizationUser = await _systemContext.OrganizationUsers
            .FirstOrDefaultAsync(x => x.UserId.Equals(userId) && x.OrganizationId.Equals(organizationId));
        
        var organization = await _systemContext.Organizations
            .Include(x=> x.Users)
            .FirstOrDefaultAsync(x => x.Id.Equals(organizationId));

        if (organizationUser is not null)
        {
            organization?.Users.Remove(organizationUser);
            var user = await _userManager.FindByIdAsync(userId);
            (user as LicencedUser)!.OrganizationUsers?.Remove(organizationUser);
            _systemContext.OrganizationUsers.Remove(organizationUser);

            await _systemContext.SaveChangesAsync();
        }
    }

    public async Task UpdateOrganization(Guid organizationId, string name, string description)
    {
        var organization = await _systemContext.Organizations
            .FirstOrDefaultAsync(x => x.Id.Equals(organizationId));

        if (organization is not null)
        {
            organization.Name = name;
            organization.Description = description;
            await _systemContext.SaveChangesAsync();
        }
    }
    
    public async Task DeleteOrganization(Guid organizationId)
    {
        var organization = await _systemContext.Organizations
            .Include(o => o.Groups)
            .FirstOrDefaultAsync(x => x.Id.Equals(organizationId));

        if (organization is not null)
        {
            var allOrganizationUsers = await _systemContext.OrganizationUsers
                .Where(x => x.OrganizationId.Equals(organizationId))
                .ToListAsync();
            
            foreach (var organizationUser in allOrganizationUsers)
                await RemoveUserFromOrganization(organizationUser.UserId, organizationId);
            
            var groups = organization?.Groups?.ToList();
            
            if (groups is not null)
            {
                foreach (var group in groups)
                    await _groupService.DeleteGroup(group.Id, organizationId);
            }
            
            _systemContext.Organizations.Remove(organization);
            await _systemContext.SaveChangesAsync();
        }
    }
}