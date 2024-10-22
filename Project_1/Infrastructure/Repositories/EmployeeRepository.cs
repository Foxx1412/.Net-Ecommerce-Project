using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Project_1.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees
                   .Include(e => e.Attendances) // Tải dữ liệu Attendance liên quan
                   .FirstOrDefaultAsync(e => e.ID_Employee == employeeId);
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
