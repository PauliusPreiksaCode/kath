using System.Security.Claims;

namespace organization_back_end.Helpers;

public static class ClaimsExtension
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.System)?.Value!;
    }
    
    public static string GetUserEmail(this ClaimsPrincipal user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        return user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value!;
    }
}