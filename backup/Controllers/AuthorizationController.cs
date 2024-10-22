using Microsoft.AspNetCore.Mvc;
using Project_1.Data;
using Project_1.Models;
using Project_1.Models.DTOs;
using Microsoft.EntityFrameworkCore; // Thêm directive này
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Project_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public AuthorizationController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // API tạo người dùng mới
        [HttpPost("create-user")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "User created successfully", user });
        }

        // API gán vai trò cho người dùng
        [HttpPost("assign-role")]
        public async Task<ActionResult> AssignRoleToUser([FromBody] UserRoleDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            var role = await _context.Roles.FindAsync(dto.RoleId);

            if (user == null || role == null)
            {
                return BadRequest(new { success = false, message = "User or Role not found" });
            }

            var userRole = new UserRole
            {
                UserId = dto.UserId,
                RoleId = dto.RoleId
            };

            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Role assigned successfully" });
        }

        
        // API kiểm tra người dùng có quyền hay không
        [HttpPost("check-permission")]
        public async Task<ActionResult> CheckPermission([FromBody] CheckPermissionDto dto)
        {
            var userRoles = await _context.UserRoles
                .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Where(ur => ur.UserId == dto.UserId)
                .ToListAsync();
            var userName = await _context.UserRoles
                .Where(ur => ur.UserId == dto.UserId)
                .Select(ur => ur.User.Username)  // Lấy ra Username từ UserRole
                .FirstOrDefaultAsync();


            var hasPermission = userRoles.Any(ur =>
                ur.Role.RolePermissions.Any(rp => rp.Permission.Id == dto.PermissionId));

            if (!hasPermission)
            {
                return StatusCode(403, new { success = false, message = "User does not have permission" });
            }

            // Tạo JWT khi có quyền
            var token = GenerateJwtToken(userName);

            return Ok(new
            {
                success = true,
                message = "User has permission",
                token = token // Trả về JWT
            });
        }

        private string GenerateJwtToken(String userId)
        {
            var jwtSettings = _configuration.GetSection("JWT");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
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
