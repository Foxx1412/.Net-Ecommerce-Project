using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Project_1.Infrastructure.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly AppDbContext _context;

        public PayrollRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPayrollAsync(Payroll payroll)
        {
            _context.Payrolls.Add(payroll);
            await _context.SaveChangesAsync();
        }

        public async Task<Payroll> GetPayrollByEmployeeIdAsync(int employeeId)
        {
            return await _context.Payrolls.FirstOrDefaultAsync(p => p.ID_Employee == employeeId);
        }

        public async Task<List<Payroll>> GetAllPayrollsAsync()
        {
            return await _context.Payrolls.ToListAsync();
        }
    }
}
