using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Project_1.NewFolder1;

namespace Project_1.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly AppDbContext _context;

        public DiscountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Discount> GetDiscountByCodeAsync(string discountCode)
        {
            var nowDate = TimeHelper.NowVietnamTime();
            return await _context.Discounts
                .FirstOrDefaultAsync(d => d.Code == discountCode && d.IsActive && nowDate > d.StartDate && nowDate < d.ExpiryDate );
        }
    }
}
