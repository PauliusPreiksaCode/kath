using organization_back_end.Auth.Model;
using organization_back_end.ResponseDto.Users;

namespace organization_back_end.Interfaces;

public interface IUserService
{
    Task<User?> GetUserById(string id);
    Task<ICollection<UserResponseDto>> GetOtherAllUsers(string userId);
}