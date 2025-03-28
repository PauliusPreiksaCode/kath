﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Group;
using organization_back_end.ResponseDto.Groups;

namespace organization_back_end.Services;

public class GroupService : IGroupService
{
    private readonly SystemContext _systemContext;
    private readonly IEntryService _entryService;
    private readonly UserManager<User> _userManager;

    public GroupService(SystemContext systemContext, IEntryService entryService, UserManager<User> userManager)
    {
        _systemContext = systemContext;
        _entryService = entryService;
        _userManager = userManager;
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
    
    public async Task<ICollection<GroupResponseDto>> GetGroups(Guid organizationId)
    {
        var groups = await _systemContext.Groups
            .Include(x => x.Entries)
            .Where(x => x.OrganizationId.Equals(organizationId))
            .Select(g => new GroupResponseDto()
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                CreationDate = g.CreationDate,
                OrganizationId = g.OrganizationId,
                EntriesCount = g.Entries != null ? g.Entries.Count : 0,
            })
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
        
        var entries = await _systemContext.Entries
            .Where(x => x.GroupId.Equals(request.GroupId))
            .ToListAsync();
        
        foreach (var entry in entries)
        {
            await _entryService.DeleteEntry(entry.Id, entry.GroupId, entry.LicencedUserId);
        }
        
        var organization = await _systemContext.Organizations
            .FirstOrDefaultAsync(x => x.Id.Equals(request.OrganizationId));
        
        organization!.Groups!.Remove(group);
        group.Organization = null;
        group.OrganizationId = Guid.Empty;
        
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