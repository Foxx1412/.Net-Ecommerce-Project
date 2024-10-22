using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
namespace Project_1.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(int pageNumber, int pageSize)
        {
            return await _context.Customers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending( c => c.created_at )
                .ToListAsync();
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<Customer> getCustomerByIdAsync(int id) {
            return await _context.Customers.FirstOrDefaultAsync(p => p.ID_User == id);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers
                                              .FirstOrDefaultAsync(c => c.ID_User == customer.ID_User);

            if (existingCustomer == null)
            {
                throw new Exception("Khách hàng không tồn tại.");
            }

            // Cập nhật các trường thông tin
            existingCustomer.FullName = customer.FullName;
            existingCustomer.Email = customer.Email;
            existingCustomer.Phone = customer.Phone;
            existingCustomer.Address = customer.Address;
            existingCustomer.updated_at = DateTime.Now;

            // Đánh dấu thực thể đã bị sửa đổi
            _context.Customers.Update(existingCustomer);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
