using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;

namespace organization_back_end.Services;

public class UserService
{
    private readonly SystemContext _context;

    public UserService(SystemContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetUserById(string id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
    } 
}