using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Project_1.Application.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace Project_1.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserRole>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .ToListAsync();
        }

        public async Task<bool> AssignRoleToUserAsync(UserRole userRole)
        {
            // Kiểm tra xem user và role có tồn tại không
            var user = await _context.Users.FindAsync(userRole.UserId);
            var role = await _context.Roles.FindAsync(userRole.RoleId);

            if (user == null || role == null)
            {
                return false; // Trả về false nếu không tìm thấy user hoặc role
            }

            // Kiểm tra xem vai trò đã được gán cho người dùng chưa
            var existingUserRole = await _context.UserRoles
                .AsNoTracking() // 
                .AnyAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId);

            if (existingUserRole)
            {
                return false; // Nếu vai trò đã được gán trước đó, không làm gì cả
            }

            // Gán vai trò cho người dùng
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return true; // Trả về true nếu việc gán vai trò thành công
        }


        public async Task<List<UserRole>> GetUserRoleWithPermissionsAsync(int userId)
        {
            return await _context.UserRoles
               .Include(ur => ur.Role) // Include Role
               .ThenInclude(r => r.RolePermissions) // Include RolePermissions
               .ThenInclude(rp => rp.Permission) // Include Permission
               .Include(ur => ur.User) // Include User
               .Where(ur => ur.UserId == userId)
               .ToListAsync();
        }

    }
}
