using Project_1.Core.Interfaces;
using Project_1.Core.Entities;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Project_1.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<int> GetOrderCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetOrderCountInTimeRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Orders.CountAsync(o => o.created_at >= from && o.created_at <= to);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders.SumAsync(o => o.TotalAmount);
        }
    }
}
