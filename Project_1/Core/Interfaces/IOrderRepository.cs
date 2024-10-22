using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync(int pageNumber, int pageSize);
        Task<Order> CreateOrderAsync(Order order);
        Task<int> GetOrderCountAsync();
        Task<int> GetOrderCountInTimeRangeAsync(DateTime from, DateTime to);
        Task<decimal> GetTotalRevenueAsync();

       
    }
}
