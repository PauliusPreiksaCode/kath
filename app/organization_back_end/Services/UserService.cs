using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.ResponseDto.Users;

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
    
    public async Task<ICollection<UserResponseDto>> GetOtherAllUsers(string userId)
    {
        return await _context.Users
            .Where(u => !u.Id.Equals(userId))
            .Select(user => new UserResponseDto()
            {
                Id = user.Id,
                Email = user.UserName!
            })
            .ToListAsync();
    } 
}