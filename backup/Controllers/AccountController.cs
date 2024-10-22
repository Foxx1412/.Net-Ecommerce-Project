using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AccountController : ControllerBase
    {
        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result.Succeeded)
            {
                // Xử lý sau khi người dùng đăng nhập thành công
                var userClaims = result.Principal.Claims;
                return Ok(new { Message = "Logged in", Claims = userClaims });
            }   
            return BadRequest("Authentication failed");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Logged out" });
        }
    }
}
