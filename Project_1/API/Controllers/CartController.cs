using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Project_1.Application.DTOs;
using Project_1.Application.Services;
using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Repositories;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        private readonly ICartRepository _cartRepository;
        private readonly DiscountService _discountService;

        public CartController(CartService cartService, ICartRepository cartRepository, DiscountService discountService)
        {
            _cartService = cartService;
            _cartRepository = cartRepository;
            _discountService = discountService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] Cart cart)
        {
            try
            {
                await _cartService.AddToCartAsync(cart);
                return Ok(new { message = "Product added to cart successfully", cart });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("apply-discount")]
        public async Task<IActionResult> ApplyDiscount([FromBody] ApplyDiscountRequestDto request)
        {
            var cart = await _cartRepository.GetCartByCustomerIdAsync(request.CustomerId);
            if (cart == null)
            {
                return NotFound("Giỏ hàng không tồn tại.");
            }

            try
            {
                // Áp dụng mã giảm giá và tính toán tổng giá trị mới
                var finalPrice = await _discountService.ApplyDiscountAsync(request.DiscountCode, cart);

                // Cập nhật tổng giá trị mới
                cart.TotalPrice = finalPrice;

                // Lấy danh sách mã giảm giá hiện tại từ giỏ hàng
                var currentDiscountCodes = cart.DiscountCode?.Split(',').Select(dc => dc.Trim()).ToList() ?? new List<string>();

                // Kiểm tra nếu mã giảm giá đã tồn tại trong giỏ hàng
                if (!currentDiscountCodes.Contains(request.DiscountCode))
                {
                    // Thêm mã mới vào danh sách mã giảm giá
                    currentDiscountCodes.Add(request.DiscountCode); // Thêm mã mới vào danh sách
                }

                // Nối mã với dấu phẩy
                cart.DiscountCode = string.Join(",", currentDiscountCodes);

                // Cập nhật giỏ hàng trong cơ sở dữ liệu
                await _cartRepository.UpdateCartAsync(cart);

                return Ok(new { finalPrice = cart.TotalPrice });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("remove-cart-item/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            try
            {
                await _cartService.RemoveCartItemByIdAsync(cartItemId);
                return Ok("Sản phẩm đã được xóa khỏi giỏ hàng.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
