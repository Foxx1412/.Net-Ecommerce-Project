using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Project_1.Models;
using Project_1.Services;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Org.BouncyCastle.Crypto.Generators;
using Project_1.Models.DTOs;

namespace Project_1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtService _jwtService;
        private readonly AppDbContext _context;
        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, [FromServices] AppDbContext _context)
        {
            // Kiểm tra xem người dùng đã tồn tại chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == registerDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Tên người dùng đã tồn tại" });
            }

            // Kiểm tra tính hợp lệ của mật khẩu
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                return BadRequest(new { message = "Mật khẩu và xác nhận mật khẩu không khớp" });
            }

            // Hash mật khẩu
            // var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            // Tạo người dùng mới
            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = registerDto.Password
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); // Lưu người dùng vào db để lấy UserId


            // Gán vai trò cho người dùng
            if (registerDto.RoleIds != null && registerDto.RoleIds.Any())
            {
                foreach (var roleId in registerDto.RoleIds)
                {
                    var userRole = new UserRole
                    {
                        UserId = newUser.Id,  // Sử dụng UserId mới tạo
                        RoleId = roleId
                    };
                    _context.UserRoles.Add(userRole);
                }
                await _context.SaveChangesAsync(); // Lưu các vai trò vào db
            }

            // Trả về kết quả đăng ký thành công
            return Ok(new { message = "Đăng ký thành công", userId = newUser.Id });
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            // Tìm kiếm người dùng trong cơ sở dữ liệu dựa trên Username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == login.Username);

            // Kiểm tra nếu không tìm thấy người dùng
            if (user == null || login.Username != user.Username || login.Password != user.PasswordHash)
            {
                return Unauthorized(new { message = "Thông tin đăng nhập không chính xác" });
            }

            // Lấy danh sách RoleId dựa vào UserId
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Include(ur => ur.Role) // Bao gồm thông tin của Role
                .ThenInclude(r => r.RolePermissions) // Bao gồm thông tin về các RolePermissions
                .ThenInclude(rp => rp.Permission) // Bao gồm thông tin về các Permissions
                .ToListAsync();

            // Lấy danh sách RoleId hoặc RoleName
            var roles = userRoles.Select(ur => new
            {
                ur.Role.Id,       // Role ID
                ur.Role.Name,      // Role Name nếu cần
                Permissions = ur.Role.RolePermissions.Select(rp => new
                {
                    rp.Permission.Id,        // Permission ID
                    rp.Permission.Name       // Permission Name
                }).ToList() // Danh sách các Permission
            }).ToList();

            // Tạo thông tin người dùng trả về
            var userInfo = new
            {
                userId = user.Id,
                username = user.Username,
                roles = roles  // Trả về mảng các role
            };

            // Tạo JWT token
            var token = GenerateJwtToken(userInfo);

            // Trả về token và thông tin người dùng cho client
            return Ok(new { token, userInfo });
        }


        private string GenerateJwtToken(object objectInfo)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, JsonConvert.SerializeObject(objectInfo)),
               
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
