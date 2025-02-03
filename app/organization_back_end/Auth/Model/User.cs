using Microsoft.AspNetCore.Identity;

namespace organization_back_end.Auth.Model;

public class User : IdentityUser
{
    public String Name { get; set; }
    public String Surname { get; set; }
}