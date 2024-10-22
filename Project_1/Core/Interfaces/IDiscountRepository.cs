using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IDiscountRepository
    {
        Task<Discount> GetDiscountByCodeAsync(string discountCode);
    }
}
