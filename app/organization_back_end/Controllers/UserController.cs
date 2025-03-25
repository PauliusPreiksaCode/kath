using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using organization_back_end.Helpers;
using organization_back_end.Interfaces;
using organization_back_end.Services;

namespace organization_back_end.Controllers;

[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    [Authorize]
    [Route("allOtherUsers")]
    public async Task<IActionResult> GetAllOtherUsers()
    {
        var userId = User.GetUserId();
        var users = await _userService.GetOtherAllUsers(userId);
        return Ok(users);
    }
}