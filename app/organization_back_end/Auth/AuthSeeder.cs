using Microsoft.AspNetCore.Identity;
using organization_back_end.Auth.Model;

namespace organization_back_end.Auth;

public class AuthSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthSeeder(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    
    public async Task SeedAsync()
    {
        await AddDefaultRoles();
    }

    private async Task AddDefaultRoles()
    {
        foreach (var role in Roles.All)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if(!roleExist)
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}