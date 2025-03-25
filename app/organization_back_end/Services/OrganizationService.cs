using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Organization;
using organization_back_end.ResponseDto.Organizations;
using organization_back_end.ResponseDto.Users;

namespace organization_back_end.Services;

public class OrganizationService : IOrganizationService
{
    private readonly SystemContext _systemContext;
    private readonly UserManager<User> _userManager;
    private readonly IGroupService _groupService;
    private readonly IEmailService _emailService;

    public OrganizationService(SystemContext systemContext, UserManager<User> userManager, IGroupService groupService, IEmailService emailService)
    {
        _systemContext = systemContext;
        _userManager = userManager;
        _groupService = groupService;
        _emailService = emailService;
    }
    
    public async Task<ICollection<OrganizationResponseDto>> GetOrganizations(string userId)
    {
        var userOrganizationsId = await _systemContext.OrganizationUsers
            .Where(u => u.UserId.Equals(userId))
            .Select(u => u.OrganizationId)
            .Distinct()
            .ToListAsync();

        var organizations = await _systemContext.Organizations
            .AsNoTracking()
            .Include(o => o.Groups)
            .Include(o => o.Users)
            .Where(o => userOrganizationsId.Contains(o.Id))
            .Select(o => new OrganizationResponseDto()
            {
                Id = o.Id,
                Name = o.Name,
                Description = o.Description,
                CreationDate = o.CreationDate,
                Groups = o.Groups,
                OwnerId = o.OwnerId,
                MembersCount = o.Users.Count
            })
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
            
            var organizationOwner = await _userManager.FindByIdAsync(organization.OwnerId);
            _emailService.SendInvitationEmail(organization.Name, organizationOwner!.Name, user.Name, user.Email!);
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

    public async Task<ICollection<UserResponseDto>> GetOrganizationUsers(Guid organizationId, string userId)
    {
        var organization = await _systemContext.Organizations
            .Include(o => o.Users).ThenInclude(organizationUser => organizationUser.User)
            .FirstOrDefaultAsync(x => x.Id.Equals(organizationId));

        if (organization is null)
            return Enumerable.Empty<UserResponseDto>().ToList();
        
        var users = organization.Users
            .Where(x => !x.UserId.Equals(userId))
            .Select(x => new UserResponseDto()
            {
                Id = x.UserId,
                Email = x.User.UserName!
            })
            .ToList();
        
        return users; 
    }
    
    public async Task<ICollection<UserResponseDto>> GetNonOrganizationUsers(Guid organizationId, string userId)
    {
        var organization = await _systemContext.Organizations
            .Include(o => o.Users).ThenInclude(organizationUser => organizationUser.User)
            .FirstOrDefaultAsync(x => x.Id.Equals(organizationId));

        if (organization is null)
            return Enumerable.Empty<UserResponseDto>().ToList();
        
        var users = await _systemContext.Users
            .Where(u => !organization.Users.Select(x => x.UserId).Contains(u.Id) && !u.Id.Equals(userId))
            .Select(u => new UserResponseDto()
            {
                Id = u.Id,
                Email = u.UserName!
            })
            .ToListAsync();
        
        return users;
    }
}