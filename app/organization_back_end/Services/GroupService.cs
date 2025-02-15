using Microsoft.EntityFrameworkCore;
using organization_back_end.Entities;
using organization_back_end.RequestDtos.Group;

namespace organization_back_end.Services;

public class GroupService
{
    private readonly SystemContext _systemContext;

    public GroupService(SystemContext systemContext)
    {
        _systemContext = systemContext;
    }

    public async Task CreateGroup(AddGroupRequest request)
    {
        var organization = await _systemContext.Organizations
            .FirstOrDefaultAsync(x => x.Id.Equals(request.OrganizationId));
        
        if (organization is null)
        {
            throw new Exception("Organization not found");
        }

        var group = new Group()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreationDate = DateTime.Now,
            OrganizationId = request.OrganizationId,
            Organization = organization,
            Entries = new List<Entry>()
        };
        
        organization.Groups ??= new List<Group>();
        organization.Groups.Add(group);
        
        await _systemContext.Groups.AddAsync(group);
        await _systemContext.SaveChangesAsync();
    }
    
    public async Task<ICollection<Group>> GetGroups(Guid organizationId)
    {
        var groups = await _systemContext.Groups
            .Where(x => x.OrganizationId.Equals(organizationId))
            .ToListAsync();
        
        return groups;
    }
    
    public async Task UpdateGroup(UpdateGroupRequest request)
    {
        var group = await _systemContext.Groups
            .FirstOrDefaultAsync(x => x.Id.Equals(request.GroupId) && x.OrganizationId.Equals(request.OrganizationId));
        
        if (group is null)
        {
            throw new Exception("Group not found");
        }

        group.Name = request.Name;
        group.Description = request.Description;
        
        _systemContext.Groups.Update(group);
        await _systemContext.SaveChangesAsync();
    }
    
    public async Task DeleteGroup(DeleteGroupRequest request)
    {
        var group = await _systemContext.Groups
            .FirstOrDefaultAsync(x => x.Id.Equals(request.GroupId) && x.OrganizationId.Equals(request.OrganizationId));
        
        if (group is null)
        {
            throw new Exception("Group not found");
        }
        
        var organization = await _systemContext.Organizations
            .FirstOrDefaultAsync(x => x.Id.Equals(request.OrganizationId));
        
        organization.Groups.Remove(group);
        _systemContext.Groups.Remove(group);
        await _systemContext.SaveChangesAsync();
    }

    public async Task DeleteGroup(Guid groupId, Guid organizationId)
    {
        var  request = new DeleteGroupRequest()
        {
            GroupId = groupId,
            OrganizationId = organizationId
        };

        await DeleteGroup(request);
    }
}