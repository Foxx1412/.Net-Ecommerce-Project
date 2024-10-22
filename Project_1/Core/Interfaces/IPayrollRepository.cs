using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IPayrollRepository
    {
        Task AddPayrollAsync(Payroll payroll);
        Task<Payroll> GetPayrollByEmployeeIdAsync(int employeeId);
        Task<List<Payroll>> GetAllPayrollsAsync();
    }
}
