using Project_1.Core.Interfaces;
using Project_1.Core.Entities;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Project_1.Infrastructure.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Wishlist>> GetWishlistsByUserId(int userId)
        {
            return await _context.Wishlist
                .Where(w => w.ID_Customer == userId)
                .ToListAsync();
        }

        public async Task AddToWishlist(Wishlist wishlist)
        {
            _context.Wishlist.Add(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromWishlist(int userId, int productId)
        {
            var wishlistItem = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.ID_Customer == userId && w.ID_Product == productId);
            if (wishlistItem != null)
            {
                _context.Wishlist.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
