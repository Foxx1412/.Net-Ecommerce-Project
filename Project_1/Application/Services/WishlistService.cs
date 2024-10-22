using Project_1.Core.Entities;
using Project_1.Core.Interfaces;

namespace Project_1.Application.Services
{
    public class WishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;

        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }

        public async Task<IEnumerable<Wishlist>> GetWishlistsForUser(int userId)
        {
            return await _wishlistRepository.GetWishlistsByUserId(userId);
        }

        public async Task AddToWishlist(int userId, int productId)
        {
            var wishlist = new Wishlist
            {
                ID_Customer = userId,
                ID_Product = productId,
            };
            await _wishlistRepository.AddToWishlist(wishlist);
        }

        public async Task RemoveFromWishlist(int userId, int productId)
        {
            await _wishlistRepository.RemoveFromWishlist(userId, productId);
        }
    }
}
