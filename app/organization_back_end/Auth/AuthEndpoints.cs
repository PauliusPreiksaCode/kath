using System.Security.Claims;
using System.Transactions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using organization_back_end.Auth.Model;
using organization_back_end.RequestDtos.Auth;
using organization_back_end.Services;

namespace organization_back_end.Auth;

[Route("auth")]
public class AuthEndpoints : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly JwtService _jwtService;
    private readonly SessionService _sessionService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LicenceService _licenceService;

    public AuthEndpoints(UserManager<User> userManager, JwtService jwtService, IHttpContextAccessor httpContextAccessor, SessionService sessionService, LicenceService licenceService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _httpContextAccessor = httpContextAccessor;
        _sessionService = sessionService;
        _licenceService = licenceService;
    }

    [HttpPost]
    [Route("registerClient")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var wrongJson = errors.Any(e => e.Contains("JSON"));
            return wrongJson ? StatusCode(400, "Wrong JSON format") : StatusCode(422, new { errors });
        }
        
        
        var user = await _userManager.FindByNameAsync(request.Email);

        if (user is not null)
            return StatusCode(422, "Username already taken");

        var newUser = new User()
        {
            Name = request.Name,
            Surname = request.Surname,
            UserName = request.Email,
            Email = request.Email
        };

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var createdUser = await _userManager.CreateAsync(newUser, request.Password);
            if (!createdUser.Succeeded)
                return StatusCode(422, "Not able to create user");

            var addToRoleResult = await _userManager.AddToRoleAsync(newUser, Roles.User);
            if (!addToRoleResult.Succeeded)
                return StatusCode(422, "Failed to assign role to user");
            scope.Complete();
        }

        return Created();
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var wrongJson = errors.Any(e => e.Contains("JSON"));
            return wrongJson ? StatusCode(400, "Wrong JSON format") : StatusCode(422, new { errors });
        }
        
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user is null)
            return NotFound("User not found");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if(!isPasswordValid)
            return StatusCode(422, "Password not correct");

        await _licenceService.ValidateLicenceExpiration(user.Id);

        var sessionId = Guid.NewGuid();
        var expiresAt = DateTime.UtcNow.AddDays(3);
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.CreateAccessToken(user.UserName!, user.Id, roles);
        var refreshToken = _jwtService.CreateRefreshToken(sessionId, user.Id, expiresAt);

        await _sessionService.createSessionAsync(sessionId, user.Id, refreshToken, expiresAt);

        var cookiesOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Expires = expiresAt,
            Secure = true
        };
        
        _httpContextAccessor?.HttpContext?.Response.Cookies.Append("RefreshToken", refreshToken, cookiesOptions);

        return Ok(accessToken);
    }

    [HttpPost]
    [Route("accessToken")]
    public async Task<IActionResult> AccessToken()
    {
        if (!_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
        {
            return StatusCode(422, "Unable to get refreshToken");
        }

        if (!_jwtService.TryParseRefreshToken(refreshToken, out var claims))
        {
            return StatusCode(422, "Unable to get claims");
        }

        var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound("User not found");
        }

        var sessionId = claims.FindFirstValue("SessionId");
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return NotFound("Session not found");
        }

        var sessionIdAsGuid = Guid.Parse(sessionId);
        if (!await _sessionService.IsSessionValidAsync(sessionIdAsGuid, refreshToken))
        {
            return StatusCode(422, "Session not valid");
        }
        
        var expiresAt = DateTime.UtcNow.AddDays(3);
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.CreateAccessToken(user.UserName!, user.Id, roles);
        var newRefreshToken = _jwtService.CreateRefreshToken(sessionIdAsGuid, user.Id, expiresAt);

        var cookiesOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Expires = expiresAt,
            Secure = true
        };
        
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("RefreshToken");
        _httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, cookiesOptions);

        await _sessionService.ExtendSessionAsync(sessionIdAsGuid, newRefreshToken, expiresAt);
        
        return Ok(accessToken);
    }
    
    [HttpPost]
    [Route("logOut")]
    public async Task<IActionResult> LogOut()
    {
        if (!_httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
        {
            return StatusCode(422, "Unable to get refreshToken");
        }

        if (!_jwtService.TryParseRefreshToken(refreshToken, out var claims))
        {
            return StatusCode(422, "Unable to get claims");
        }

        var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound("User not found");
        }

        var sessionId = claims.FindFirstValue("SessionId");
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return NotFound("Session not found");
        }

        await _sessionService.InvalidateSessionAsync(Guid.Parse(sessionId));
        
        var cookiesOptions = new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.None,
            Secure = true
        };
        
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("RefreshToken", cookiesOptions);
        
        return Ok();
    }
}