using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetCartByCustomerIdAsync(int customerId);

        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(Cart cart);

        Task DeleteCartItemByIdAsync(int cartItemId);
    }
}
