using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.RequestDtos.Entry;
using File = organization_back_end.Entities.File;

namespace organization_back_end.Services;

public class EntryService
{
    private readonly SystemContext _systemContext;
    private readonly UserManager<User> _userManager;
    private readonly FileService _fileService;
    
    public EntryService(SystemContext systemContext, UserManager<User> userManager, FileService fileService)
    {
        _systemContext = systemContext;
        _userManager = userManager;
        _fileService = fileService;
    }

    public async Task<ICollection<Entry>> GetEntries(Guid organizationId, Guid groupId)
    {
        var entries = await _systemContext.Entries
            .Include(e => e.File)
            .Include(e => e.Group)
            .Where(e => e.GroupId.Equals(groupId) && e.Group.OrganizationId.Equals(organizationId))
            .ToListAsync();
        
        return entries;
    }

    public async Task<FileStreamResult?> DownloadFile(Guid groupId, Guid entryId)
    {
        var entry = await _systemContext.Entries
            .Include(e => e.File)
            .FirstOrDefaultAsync(e => e.Id.Equals(entryId) && e.GroupId.Equals(groupId));

        if (entry is not null)
        {
            return await _fileService.DownloadFileAsync(entry.File!.Path);
        }

        return null;
    }

    
    public async Task CreateEntry(AddEntryRequest request, string licencedUserId)
    {
        var group = await _systemContext.Groups
            .FirstOrDefaultAsync(x => x.Id.Equals(request.GroupId) && x.OrganizationId.Equals(request.OrganizationId));
        
        if (group is null)
        {
            throw new Exception("Group not found");
        }
        
        var entry = new Entry()
        {
            Id = Guid.NewGuid(),
            Text = request.Text,
            CreationDate = DateTime.Now,
            ModifyDate = DateTime.Now,
            GroupId = request.GroupId,
            Group = group,
            LicencedUserId = licencedUserId
        };
        
        var user = await _userManager.FindByIdAsync(licencedUserId);
        var licencedUser = user as LicencedUser;
        
        licencedUser!.Entries ??= new List<Entry>();
        licencedUser.Entries.Add(entry);
        
        group.Entries ??= new List<Entry>();
        group.Entries.Add(entry);
        
        await _systemContext.Entries.AddAsync(entry);
        
        if(request.File is not null)
        {
            var guid = Guid.NewGuid();
            var url = await _fileService.UploadFileAsync(request.File, request, guid);
            
            File file = new File()
            {
                Id = guid,
                Name = request.Name,
                Extension = request.Extension,
                UploadDate = DateTime.Now,
                Path = $"{group.OrganizationId}/{group.Id}/{entry.Id}/{guid}",
                EntryId = entry.Id,
                Entry = entry,
                Url = url
            };
            
            entry.File = file;
            entry.FileId = file.Id;
            await _systemContext.Files.AddAsync(file);
        }
        
        await _systemContext.SaveChangesAsync();
    }

    public async Task UpdateEntry(UpdateEntryRequest request, string userId)
    {
        var entry = await _systemContext.Entries
            .FirstOrDefaultAsync(x => x.Id.Equals(request.EntryId) && x.GroupId.Equals(request.GroupId));

        if (entry is null)
        {
            throw new Exception("Entry not found");
        }
        
        if (!entry.LicencedUserId.Equals(userId))
        {
            throw new Exception("You are not the creator of this entry");
        }

        entry.Text = request.Text;
        entry.ModifyDate = DateTime.Now;

        await _systemContext.SaveChangesAsync();
    }
    
    public async Task DeleteEntry(Guid entryId, Guid groupId, string userId)
    {
        var entry = await _systemContext.Entries
            .Include(e => e.File)
            .FirstOrDefaultAsync(x => x.Id.Equals(entryId) && x.GroupId.Equals(groupId));
        
        if (entry is null)
        {
            throw new Exception("Entry not found");
        }
        
        var group = await _systemContext.Groups
            .FirstOrDefaultAsync(x => x.Id.Equals(groupId));
        
        if (!entry.LicencedUserId.Equals(userId))
        {
            throw new Exception("You are not the creator of this entry");
        }
        
        var user = await _userManager.FindByIdAsync(userId);
        (user as LicencedUser)!.Entries!.Remove(entry);
        group!.Entries!.Remove(entry);
        
        _systemContext.Entries.Remove(entry);
        await _systemContext.SaveChangesAsync();
        
        if(entry.FileId is not null)
        {
            var file = await _systemContext.Files.FirstOrDefaultAsync(e => e.Id.Equals(entry.FileId));
            
            string fileName = $"{file!.Id}-{file.Name}{file.Extension}";
            await _fileService.DeleteFileAsync(fileName);
            
            entry.File = null;
            entry.FileId = null;
            
            _systemContext.Files.Remove(file);
            await _systemContext.SaveChangesAsync();
        }
    }


}