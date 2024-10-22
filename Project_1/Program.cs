using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Project_1.Infrastructure.Data;
using Project_1.Application.Services;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Repositories;
// using Project_1.Services;

var builder = WebApplication.CreateBuilder(args);


// Đọc cấu hình VNPay từ appsettings.json
// builder.Services.Configure<VnpayConfig>(builder.Configuration.GetSection("VNPAY"));

// Đăng ký repository và service
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
// uilder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();  

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<CategoryService>(); 

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<CustomerService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<OrderService>();

builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<WishlistService>();

builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<TagService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<CartService>();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<DiscountService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<EmployeeService>();

builder.Services.AddScoped<IPayrollRepository, PayrollRepository>();
builder.Services.AddScoped<PayrollService>();

builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<AttendanceService>();










// Thêm dịch vụ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
}); 

// Cấu hình JWT từ appsettings.json
var jwtSettings = builder.Configuration.GetSection("JWT");

// Thêm cấu hình từ file appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Thêm dịch vụ vào container
builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});

builder.Services.AddTransient<EmailService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cấu hình kết nối MySQL
var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Cấu hình Google Authentication
builder.Services.AddAuthentication(options =>
{
    /*options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; */
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/api/account/login";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
})


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
});




var app = builder.Build();


// Sử dụng chính sách CORS
app.UseCors("AllowAllOrigins"); 
// Cấu hình pipeline HTTP request
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Đảm bảo gọi trước UseAuthorization
app.UseAuthorization();

app.MapControllers();
app.Run();
