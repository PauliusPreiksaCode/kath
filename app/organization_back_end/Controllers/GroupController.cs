using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using organization_back_end.Auth.Model;
using organization_back_end.Helpers;
using organization_back_end.RequestDtos.Group;
using organization_back_end.Services;

namespace organization_back_end.Controllers;

[Route("groups")]
public class GroupController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly GroupService _groupService;
    private readonly LicenceService _licenceService;
    private readonly OrganizationService _organizationService;

    public GroupController(UserManager<User> userManager, GroupService groupService, LicenceService licenceService, OrganizationService organizationService)
    {
        _userManager = userManager;
        _groupService = groupService;
        _licenceService = licenceService;
        _organizationService = organizationService;
    }
    
    [HttpGet]
    [Authorize]
    [Route("allGroups/{id:guid}")]
    public async Task<IActionResult> GetGroups([FromRoute] Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            
            if(await _licenceService.HasRole(userId, Roles.OrganizationOwner) ||
               await _licenceService.HasRole(userId, Roles.LicencedUser))
            {
                var groups = await _groupService.GetGroups(id);
                return Ok(groups);
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot get groups");
        }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateGroup([FromBody] AddGroupRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.OrganizationId);

            if (isOwner)
            {
                await _groupService.CreateGroup(request);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot create group");
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateGroup([FromBody] UpdateGroupRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.OrganizationId);

            if (isOwner)
            {
                await _groupService.UpdateGroup(request);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot update group");
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteGroup([FromBody] DeleteGroupRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.OrganizationId);

            if (isOwner)
            {
                await _groupService.DeleteGroup(request);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot delete group");
        }
    }
}