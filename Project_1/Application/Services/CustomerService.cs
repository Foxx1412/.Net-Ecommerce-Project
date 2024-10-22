using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Repositories;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(int pageNumber, int pageSize)
        {
            return await _customerRepository.GetAllCustomersAsync(pageNumber, pageSize);
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.getCustomerByIdAsync(id);
        }



        public async Task AddCustomerAsync(Customer customer)
        {
            customer.created_at = TimeHelper.NowVietnamTime();
            customer.updated_at = TimeHelper.NowVietnamTime();

            await _customerRepository.AddCustomerAsync(customer);
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            // Tìm khách hàng dựa trên ID_User
            var checkCustomer = await _customerRepository.getCustomerByIdAsync(customer.ID_User);
            if (customer == null)
            {
                throw new Exception("Khách hàng không tồn tại.");
            }

            // Cập nhật các thông tin cần thiết
            customer.FullName = customer.FullName;
            customer.Email = customer.Email;
            customer.Phone = customer.Phone;
            customer.Address = customer.Address;

            // Gọi repository để cập nhật khách hàng
            await _customerRepository.UpdateCustomerAsync(customer);
        }
    

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.getCustomerByIdAsync(id);
            if (customer == null)
            {
                return false;
            }

            await _customerRepository. DeleteCustomerAsync(id);
            return true;
        }
    }
}
