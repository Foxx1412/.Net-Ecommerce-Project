using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.Services;
using Project_1.Core.Entities;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly WishlistService _wishlistService;
       

        public WishlistController(WishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlists(int userId)
        {
            var wishlists = await _wishlistService.GetWishlistsForUser(userId);
            if (wishlists == null)
            {
                return NotFound();
            }
            return Ok(wishlists);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist(Wishlist wishlistDto)
        {
            // Validate input
            if (wishlistDto == null || wishlistDto.ID_Customer <= 0)
            {
                return BadRequest("Invalid wishlist data");
            }

            // Fetch user from the database using the userId
            /* var user = await _userService.GetUserByIdAsync(wishlistDto.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            } */


            await _wishlistService.AddToWishlist(wishlistDto.ID_Customer, wishlistDto.ID_Product);

            return Ok( new { message = "Product added to wishlist successfully" });
        }


        [HttpDelete("{userId}/remove/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int userId, int productId)
        {
            await _wishlistService.RemoveFromWishlist(userId, productId);
            return Ok( new { message = "Deleted successfull " });
        }
    }
}
