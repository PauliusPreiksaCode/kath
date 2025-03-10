using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Helpers;
using organization_back_end.RequestDtos.Entry;
using organization_back_end.ResponseDto.Entries;
using File = organization_back_end.Entities.File;

namespace organization_back_end.Services;

public class EntryService
{
    private readonly SystemContext _systemContext;
    private readonly UserManager<User> _userManager;
    private readonly FileService _fileService;
    private readonly IHubContext<EntriesHub> _hubContext;
    
    public EntryService(SystemContext systemContext, UserManager<User> userManager, FileService fileService, IHubContext<EntriesHub> hubContext)
    {
        _systemContext = systemContext;
        _userManager = userManager;
        _fileService = fileService;
        _hubContext = hubContext;
    }

    public async Task<ICollection<EntryResponseDto>> GetEntries(Guid organizationId, Guid groupId)
    {
        var entries = await _systemContext.Entries
            .Include(e => e.File)
            .Include(e => e.Group)
            .Include(e => e.LicencedUser)
            .Where(e => e.GroupId.Equals(groupId) && e.Group.OrganizationId.Equals(organizationId))
            .Select(e => new EntryResponseDto()
            {
                Id = e.Id,
                Name = e.Name,
                Text = e.Text,
                CreationDate = e.CreationDate,
                ModifyDate = e.ModifyDate,
                FileId = e.FileId,
                File = e.File,
                FullName = e.LicencedUser.Name + " " + e.LicencedUser.Surname,
                LicencedUserId = e.LicencedUserId
            })
            .OrderByDescending(e => e.CreationDate)
            .ToListAsync();
        
        return entries;
    }

    public async Task<FileStreamResult> DownloadFile(Guid groupId, Guid entryId)
    {
        var entry = await _systemContext.Entries
            .Include(e => e.File)
            .FirstOrDefaultAsync(e => e.Id.Equals(entryId) && e.GroupId.Equals(groupId));

        if (entry is not null)
        {
            string fileName = $"{entry.File!.Id}-{entry.File.Name}{entry.File.Extension}";
            return await _fileService.DownloadFileAsync(fileName);
        }

        throw new Exception("File not found");
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
            Name = request.EntryName,
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
        await _hubContext.Clients.Group(group!.OrganizationId.ToString()).SendAsync("UpdateEntries", group!.OrganizationId.ToString());
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

        entry.Name = request.EntryName;
        entry.Text = request.Text;
        entry.ModifyDate = DateTime.Now;

        await _systemContext.SaveChangesAsync();
        
        var group = await _systemContext.Groups
            .FirstOrDefaultAsync(x => x.Id.Equals(request.GroupId));
        
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
                Path = $"{group!.OrganizationId}/{group.Id}/{entry.Id}/{guid}",
                EntryId = entry.Id,
                Entry = entry,
                Url = url
            };
            
            entry.File = file;
            entry.FileId = file.Id;
            await _systemContext.Files.AddAsync(file);
            await _systemContext.SaveChangesAsync();
        }
        await _hubContext.Clients.Group(group!.OrganizationId.ToString()).SendAsync("UpdateEntries", group!.OrganizationId.ToString());
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
        await _hubContext.Clients.Group(group!.OrganizationId.ToString()).SendAsync("UpdateEntries", group!.OrganizationId.ToString());
    }

    public async Task DeleteFile(Guid entryId, Guid groupId, string userId)
    {
        var entry = await _systemContext.Entries
            .Include(e => e.File)
            .FirstOrDefaultAsync(x => x.Id.Equals(entryId) && x.GroupId.Equals(groupId));

        if (entry is null)
        {
            throw new Exception("Entry not found");
        }

        if (!entry.LicencedUserId.Equals(userId))
        {
            throw new Exception("You are not the creator of this entry");
        }

        var file = await _systemContext.Files.FirstOrDefaultAsync(e => e.Id.Equals(entry.FileId));

        if (file is null)
        {
            throw new Exception("File not found");
        }
        
        string fileName = $"{file.Id}-{file.Name}{file.Extension}";
        await _fileService.DeleteFileAsync(fileName);
        
        entry.File = null;
        entry.FileId = null;
        
        _systemContext.Files.Remove(file);
        await _systemContext.SaveChangesAsync();
        
        var group = await _systemContext.Groups
            .FirstOrDefaultAsync(x => x.Id.Equals(groupId));
        
        await _hubContext.Clients.Group(group!.OrganizationId.ToString()).SendAsync("UpdateEntries", group!.OrganizationId.ToString());
    }
}