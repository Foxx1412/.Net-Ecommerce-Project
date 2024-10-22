using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(employeeId);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllEmployeesAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            employee.created_at = TimeHelper.NowVietnamTime();
            await _employeeRepository.AddEmployeeAsync(employee);
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.UpdateEmployeeAsync(employee);
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            await _employeeRepository.DeleteEmployeeAsync(employeeId);
        }
    }
}
