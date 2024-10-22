using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<Wishlist>> GetWishlistsByUserId(int userId);
        Task AddToWishlist(Wishlist wishlist);
        Task RemoveFromWishlist(int userId, int productId);
    }
}
