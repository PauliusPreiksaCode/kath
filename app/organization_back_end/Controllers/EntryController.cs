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
            
            return Forbid();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("download-photo/{groupId:guid}/{entryId:guid}")]
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

            return Forbid();
        }
        catch (Exception e)
        {
            return NotFound();
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

            return Forbid();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
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

            return Forbid();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
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

            return Forbid();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
        }
    }
    
    [HttpDelete]
    [Authorize]
    [Route("delete-photo/{groupId:guid}/{entryId:guid}")]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid groupId, [FromRoute] Guid entryId)
    {
        try
        {
            var userId = User.GetUserId();

            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                await _entryService.DeleteFile(groupId, entryId, userId);
                return Ok();
            }

            return Forbid();
        }
        catch (Exception e)
        {
            return e.Message switch
            {
                "Entry not found" => NotFound(),
                "You are not the creator of this entry" => Forbid(),
                "File not found" => NotFound(),
                _ => StatusCode(400, e.Message)
            };
        }
    }
    
}