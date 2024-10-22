using Project_1.Core.Entities;

namespace Project_1.Infrastructure.Repositories
{
    public interface ICustomerRepository
    {
        Task AddCustomerAsync(Customer customer);
    }
}
