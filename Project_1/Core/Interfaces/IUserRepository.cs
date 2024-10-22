using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string username);
        Task AddUserAsync(User user);
        Task<List<UserRole>> GetUserRolesAsync(int userId);

        Task<bool> AssignRoleToUserAsync(UserRole dto);
        Task<List<UserRole>> GetUserRoleWithPermissionsAsync(int userId);
    }
}
