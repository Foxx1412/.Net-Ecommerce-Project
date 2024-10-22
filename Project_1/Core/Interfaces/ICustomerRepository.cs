using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync(int pageNumber, int pageSize);
        Task AddCustomerAsync(Customer customer);

        Task<Customer> getCustomerByIdAsync(int id);

        Task UpdateCustomerAsync(Customer customer);

        Task DeleteCustomerAsync(int id);


    }
}
