using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Helpers;
using organization_back_end.RequestDtos.Organization;
using organization_back_end.Services;

namespace organization_back_end.Controllers;

[Route("organizations")]
public class OrganizationController : ControllerBase
{
    private readonly OrganizationService _organizationService;
    private readonly UserManager<User> _userManager;
    private readonly LicenceService _licenceService;
    private readonly SystemContext _context;


    public OrganizationController(OrganizationService organizationService, UserManager<User> userManager,
        LicenceService licenceService, SystemContext context)
    {
        _organizationService = organizationService;
        _userManager = userManager;
        _licenceService = licenceService;
        _context = context;
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

                await _organizationService.CreateOrganization(userId, request, organizationUser);
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
    public async Task<IActionResult> UpdateOrganization([FromBody] UpdateOrganizationRequest request)
    {
        try
        {
            var userId = User.GetUserId();
            var isOwner = await _organizationService.IsUserOrganizationOwner(userId, request.Id);
            if (!isOwner)
                return Forbid();

            await _organizationService.UpdateOrganization(request.Id, request.Name, request.Description);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
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
                return Forbid();

            await _organizationService.DeleteOrganization(request.Id);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
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
                return Forbid();
            
            if(!(await _licenceService.HasRole(request.userId, Roles.LicencedUser) ||
               await _licenceService.HasRole(request.userId, Roles.OrganizationOwner)))
                return Forbid();

            await _organizationService.AddUserToOrganization(request.userId, request.organizationId);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
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
                return Forbid();

            await _organizationService.RemoveUserFromOrganization(request.userId, request.organizationId);
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(400, e.Message);
        }
    }
}
