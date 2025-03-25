using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Helpers;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Organization;
using organization_back_end.Services;

namespace organization_back_end.Controllers;

[Route("organizations")]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _organizationService;
    private readonly UserManager<User> _userManager;
    private readonly ILicenceService _licenceService;


    public OrganizationController(IOrganizationService organizationService, UserManager<User> userManager,
        ILicenceService licenceService)
    {
        _organizationService = organizationService;
        _userManager = userManager;
        _licenceService = licenceService;
    }

    [HttpGet]
    [Authorize]
    [Route("allOrganizations")]
    public async Task<IActionResult> GetOrganizations()
    {
        var userId = User.GetUserId();
        var organizations = await _organizationService.GetOrganizations(userId);
        return Ok(organizations);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrganization([FromBody] CreateOrganizationRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var user = await _userManager.FindByIdAsync(userId);

            if (await _licenceService.HasRole(userId, Roles.OrganizationOwner))
            {
                var organizationUser = user as OrganizationOwner;

                await _organizationService.CreateOrganization(userId, request, organizationUser!);
                return Ok();
            }

            return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot create organization");
        }
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateOrganization([FromBody] UpdateOrganizationRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.Id);
            if (!isOwner)
                return StatusCode(StatusCodes.Status403Forbidden, "User is not organization owner");

            await _organizationService.UpdateOrganization(request.Id, request.Name, request.Description);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot update organization");
        }
    }

    [HttpDelete]
    [Authorize]
    public async Task<IActionResult> DeleteOrganization([FromBody] DeleteOrganizationRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.Id);
            if (!isOwner)
                return StatusCode(StatusCodes.Status403Forbidden, "User is not organization owner");

            await _organizationService.DeleteOrganization(request.Id);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot delete organization");
        }
    }

    [HttpPost]
    [Authorize]
    [Route("addUser")]
    public async Task<IActionResult> AddUserToOrganization([FromBody] AddUserToOrganizationRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.organizationId);
            if (!isOwner)
                return StatusCode(StatusCodes.Status403Forbidden, "User is not organization owner");
            
            if(!(await _licenceService.HasRole(request.userId, Roles.LicencedUser) ||
               await _licenceService.HasRole(request.userId, Roles.OrganizationOwner)))
                return StatusCode(StatusCodes.Status403Forbidden, "User does not have a licence");

            await _organizationService.AddUserToOrganization(request.userId, request.organizationId);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot add user to organization");
        }
    }
    
    [HttpPost]
    [Authorize]
    [Route("removeUser")]
    public async Task<IActionResult> RemoveUserFromOrganization([FromBody] RemoveUserFromOrganizationRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.organizationId);
            if (!isOwner)
                return StatusCode(StatusCodes.Status403Forbidden, "User is not organization owner");

            await _organizationService.RemoveUserFromOrganization(request.userId, request.organizationId);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot remove user from organization");
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("organizationUsers/{id:guid}")]
    public async Task<IActionResult> GetOrganizationUsers([FromRoute] Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, id);
            if (!isOwner)
                return StatusCode(StatusCodes.Status403Forbidden, "User is not organization owner");
            
            var users = await _organizationService.GetOrganizationUsers(id, userId);
            return Ok(users);
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot get organization users");
        }
    }
    
    [HttpGet]
    [Authorize]
    [Route("nonOrganizationUsers/{id:guid}")]
    public async Task<IActionResult> GetNonOrganizationUsers([FromRoute] Guid id)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, id);
            if (!isOwner)
                return StatusCode(StatusCodes.Status403Forbidden, "User is not organization owner");
            
            var users = await _organizationService.GetNonOrganizationUsers(id, userId);
            return Ok(users);
        }
        catch (Exception e)
        {
            return StatusCode(400, "Cannot get non organization users");
        }
    }
}
