using Project_1.Core.Interfaces;
using Project_1.Core.Entities;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Project_1.Application.Services;
namespace Project_1.Infrastructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartByCustomerIdAsync(int customerId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.CustomerID == customerId);
        }

        public async Task AddCartAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartAsync(Cart cart)
        {
            var existingCart = await _context.Carts.FirstOrDefaultAsync(c => c.CustomerID == cart.CustomerID);

            if (existingCart == null)
            {
                throw new Exception("Cart does not exist.");
            }

            existingCart.TotalPrice = cart.TotalPrice;
            existingCart.DiscountCode = cart.DiscountCode;
            existingCart.created_at = DateTime.UtcNow;

            _context.Carts.Update(existingCart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCartItemByIdAsync(int cartItemId)
        {
            // Tìm cart item theo ID
            var cartItem = await _context.CartItems.FindAsync(cartItemId);

            if (cartItem == null)
            {
                throw new Exception("CartItem không tồn tại.");
            }

            // Lưu cartId để xóa cart sau này
            int cartId = cartItem.CartId;

            // Xóa cart item
            _context.CartItems.Remove(cartItem);

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            // Cập nhật giá trị tổng của cart
            var cart = await _context.Carts.FindAsync(cartId);
            if (cart != null)
            {
                // Tính toán lại tổng giá trị giỏ hàng
                cart.TotalPrice = await _context.CartItems
                    .Where(ci => ci.CartId == cartId)
                    .SumAsync(ci => ci.Quantity * ci.UnitPrice);

                // Nếu không còn cart items nào, xóa cart
                if (!await _context.CartItems.AnyAsync(ci => ci.CartId == cartId))
                {
                    _context.Carts.Remove(cart);
                }
                else
                {
                    // Lưu lại thay đổi cho cart
                    _context.Carts.Update(cart);
                }

                // Lưu thay đổi
                await _context.SaveChangesAsync();
            }
        }





    }

}
