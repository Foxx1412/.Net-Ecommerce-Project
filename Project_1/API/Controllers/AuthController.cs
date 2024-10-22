using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Application.DTOs;
using Project_1.Application.Services;
using Project_1.Core.Entities;
using Project_1.Infrastructure.Data;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
       

        public AuthController(AuthService authService, AppDbContext context)
        {
            _authService = authService;
           
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Gọi phương thức Register từ AuthService, nó sẽ xử lý cả việc đăng ký và gán vai trò
                var result = await _authService.RegisterAsync(registerDto);

                // Trả về kết quả thành công
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về lỗi
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            try
            {
                var result = await _authService.LoginAsync(login);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("check-permission")]
        public async Task<ActionResult> CheckPermission([FromBody] CheckPermissionDto dto)
        {
            var (hasPermission, token) = await _authService.CheckPermissionAsync(dto.UserId, dto.PermissionId);

            if (!hasPermission)
            {
                return StatusCode(403, new { success = false, message = "User does not have permission" });
            }

            return Ok(new
            {
                success = true,
                message = "User has permission",
                token = token
            });
        }
    }
}
