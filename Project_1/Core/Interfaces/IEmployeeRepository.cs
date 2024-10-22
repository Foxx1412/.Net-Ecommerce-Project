using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetEmployeeByIdAsync(int employeeId);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int employeeId);
    }
}
