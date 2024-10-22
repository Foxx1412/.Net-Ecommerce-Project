using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Generators;
using Project_1.Application.DTOs;
using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project_1.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly CustomerService _customerService;
        private readonly EmployeeService _employeeService;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, CustomerService customerService, EmployeeService employeeService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _customerService = customerService;
            _employeeService = employeeService;
        }

        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            // Kiểm tra xem người dùng đã tồn tại chưa
            var existingUser = await _userRepository.GetUserByUsernameAsync(registerDto.Username);
            if (existingUser != null)
            {
                throw new Exception("Tên người dùng đã tồn tại.");
            }

            // Kiểm tra tính hợp lệ của mật khẩu
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new Exception("Mật khẩu và xác nhận mật khẩu không khớp.");
            }

            // Tạo người dùng mới
            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = registerDto.Password
            };

            await _userRepository.AddUserAsync(newUser);

            // Gán vai trò cho người dùng
            if (registerDto.RoleIds != null && registerDto.RoleIds.Any())
            {
                foreach (var roleId in registerDto.RoleIds)
                {
                    // Chuyển đổi từ DTO sang thực thể UserRole
                    var userRole = new UserRole
                    {
                        UserId = newUser.Id,
                        RoleId = roleId
                    };

                    // Gọi phương thức AssignRoleToUserAsync với thực thể UserRole
                    await _userRepository.AssignRoleToUserAsync(userRole);
                }
            }

            // Nếu role khác 4 (không phải khách hàng) thì thêm vào bảng Employee
            if (registerDto.RoleIds != null && !registerDto.RoleIds.Contains(4))
            {
                var newEmployee = new Employee
                {
                    ID_Employee = newUser.Id,
                    FullName = registerDto.Fullname,
                    Email = "",
                    Phone = "",
                    BaseSalary = 0, // Có thể gán lương mặc định hoặc để trống nếu chưa có
                    Attendances = new List<Attendance>(),
                    Payrolls = new List<Payroll>()
                };

                await _employeeService.AddEmployeeAsync(newEmployee);
            }
            else
            {
                // Nếu vai trò là 4 (khách hàng), thêm vào bảng Customer
                var newCustomer = new Customer
                {
                    ID_User = newUser.Id,
                    FullName = registerDto.Fullname,
                    Email = "",
                    Phone = "",
                    Address = ""
                };

                await _customerService.AddCustomerAsync(newCustomer);
            }

            return "Đăng ký thành công";
        }



        public async Task<object> LoginAsync(Login login)
        {
            // Tìm kiếm người dùng dựa trên Username
            var user = await _userRepository.GetUserByUsernameAsync(login.Username);
            if (user == null || user.PasswordHash != login.Password)
            {
                throw new Exception("Thông tin đăng nhập không chính xác.");
            }

            // Lấy danh sách Role và Permission của người dùng
            var userRoles = await _userRepository.GetUserRolesAsync(user.Id);
            var roles = userRoles.Select(ur => new
            {
                ur.Role.Id,
                ur.Role.Name,
                Permissions = ur.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList()
            }).ToList();

            // Tạo thông tin trả về
            var userInfo = new
            {
                userId = user.Id,
                username = user.Username,
                roles = roles
            };

            // Tạo JWT token
            var token = GenerateJwtToken(userInfo);

            return new { token, userInfo };
        }

        public async Task<(bool, string)> CheckPermissionAsync(int userId, int permissionId)
        {
            // Get the user's roles with permissions
            var userRoles = await _userRepository.GetUserRoleWithPermissionsAsync(userId);

            var hasPermission = userRoles.Any(ur =>
                ur.Role.RolePermissions.Any(rp => rp.Permission.Id == permissionId));

            if (!hasPermission)
            {
                return (false, null);
            }

            // Safely access the User's Username
            var userName = userRoles.FirstOrDefault()?.User?.Username;

            // Check if userName is null
            if (string.IsNullOrEmpty(userName))
            {
                return (false, null); // Handle case where username is not found
            }

            var objectInfo = new { userId = userId, username = userName };

            // Generate JWT token if the user has permission
            var token = GenerateJwtToken(objectInfo);

            return (true, token);
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
