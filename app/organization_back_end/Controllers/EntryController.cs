using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using organization_back_end.Auth.Model;
using organization_back_end.Helpers;
using organization_back_end.RequestDtos.Entry;
using organization_back_end.Services;

namespace organization_back_end.Controllers;

[Route("entries")]
public class EntryController : ControllerBase
{
    private readonly EntryService _entryService;
    private readonly LicenceService _licenceService;


    public EntryController(EntryService entryService, LicenceService licenceService)
    {
        _entryService = entryService;
        _licenceService = licenceService;
    }
    
    [HttpGet]
    [Authorize]
    [Route("allEntries/{organizationId:guid}/{groupId:guid}")]
    public async Task<IActionResult> GetEntries([FromRoute] Guid organizationId, [FromRoute] Guid groupId)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                var groups = await _entryService.GetEntries(organizationId, groupId);
                return Ok(groups);
            }
            
            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot get entries");
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("download-file/{groupId:guid}/{entryId:guid}")]
    public async Task<IActionResult> DownloadFile([FromRoute] Guid groupId, [FromRoute] Guid entryId)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                var file = await _entryService.DownloadFile(groupId, entryId);
                return file;
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return NotFound("File not found");
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateEntry([FromForm] AddEntryRequest request)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                await _entryService.CreateEntry(request, userId);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot create entry");
        }
    }
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateEntry([FromForm] UpdateEntryRequest request)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                await _entryService.UpdateEntry(request, userId);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot update entry");
        }
    }
    
    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteEntry([FromBody] DeleteEntryRequest request)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                await _entryService.DeleteEntry(request.EntryId, request.GroupId, userId);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot delete entry");
        }
    }
    
    [HttpDelete]
    [Authorize]
    [Route("delete-file/{groupId:guid}/{entryId:guid}")]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid groupId, [FromRoute] Guid entryId)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                await _entryService.DeleteFile(entryId, groupId, userId);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Entry not found" => NotFound("Entry not found"),
                "You are not the creator of this entry" => StatusCode(StatusCodes.Status403Forbidden, "You are not the creator of this entry"),
                "File not found" => NotFound("File not found"),
                _ => StatusCode(400, "Cannot delete file")
            };
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("linkingEntries/{organizationId:guid}")]
    public async Task<IActionResult> GetLinkingEntries([FromRoute] Guid organizationId, [FromQuery] Guid? entryToExclude = null)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                var entries = await _entryService.LinkingEntries(organizationId, entryToExclude);
                return Ok(entries);
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot get linking entries");
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("graphEntries/{organizationId:guid}")]
    public async Task<IActionResult> GetGraphEntries([FromRoute] Guid organizationId)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                var entries = await _entryService.GetGraphEntities(organizationId);
                return Ok(entries);
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot get graph entries");
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("{organizationId:guid}/{entryId:guid}")]
    public async Task<IActionResult> GetEntry([FromRoute] Guid organizationId, [FromRoute] Guid entryId)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                var entry = await _entryService.GetEntry(entryId, organizationId);
                return Ok(entry);
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot get entry");
        }
    }
    
}