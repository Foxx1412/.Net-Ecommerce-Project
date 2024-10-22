using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetRoleByIdAsync(int roleId);
    }
}
