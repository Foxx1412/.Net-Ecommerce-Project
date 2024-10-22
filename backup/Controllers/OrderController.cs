using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Models.DTOs;
using Project_1.Models;
using Project_1.Data;
using Project_1.NewFolder1;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;

namespace Project_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int pageNumber = 1, int pageSize = 10)
        {
            // Tính toán ngày hiện tại
            var today = TimeHelper.NowVietnamTime().Date;

            // Lấy tổng số đơn hàng và đơn hàng theo phân trang
            var ordersQuery = _context.Orders.Include(o => o.Customer);
            var totalOrders = await ordersQuery.CountAsync();

            var orders = await ordersQuery
                .OrderByDescending(o => o.created_at) // Sắp xếp theo created_at từ sớm nhất đến muộn nhất
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.OrderID,
                    p.Customer.FullName,
                    p.TotalAmount,
                    // Category = new { p.Category.Id, p.Category.Name }
                })
                .ToListAsync();

            // Lấy danh sách đơn hàng trong ngày hôm nay
            var ordersTodayCount = await ordersQuery
                .CountAsync(o => o.created_at >= today && o.created_at < today.AddDays(1));

            var ordersToday = await ordersQuery
                .Where(o => o.created_at >= today && o.created_at < today.AddDays(1))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.OrderID,
                    p.Customer.FullName,
                    p.TotalAmount,
                    // Category = new { p.Category.Id, p.Category.Name }
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalOrders / (double)pageSize);

            var result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalOrders = totalOrders,
                Orders = orders,
                OrdersToday = ordersToday
            };

            return Ok(new { orders = result });
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderDto orderDto)
        {
            // Kiểm tra khách hàng có tồn tại hay không
            var customer = await _context.Customers.FindAsync(orderDto.CustomerId);
            if (customer == null)
            {
                return BadRequest(new { message = "Customer not found." });
            }

            // Sinh mã OrderID tự động (VD: O-{yyyyMMddHHmmss}-{4 số ngẫu nhiên})
            var orderID = $"O-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

            // Tạo đối tượng Order
            var order = new Order
            {
                CustomerID = orderDto.CustomerId,
                OrderID = orderID,  // Gán mã OrderID tự động
                Status = "Pending",
                created_at = TimeHelper.NowVietnamTime(),
                updated_at = TimeHelper.NowVietnamTime(),
                OrderItems = new List<OrderItems>()
            };

            // Danh sách để lưu trữ thông tin các sản phẩm
            var orderItems = new List<OrderItems>();
            decimal totalAmount = 0;

            foreach (var itemDto in orderDto.OrderItems)
            {
                // Lấy thông tin sản phẩm từ database
                var product = await _context.Products.FindAsync(itemDto.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = $"Product with ID {itemDto.ProductId} not found." });
                }

                // Kiểm tra xem sản phẩm có đủ số lượng không
                if (product.Quantity < itemDto.Quantity)
                {
                    return BadRequest(new { message = $"Not enough quantity for product ID {itemDto.ProductId}." });
                }

                // Tính tổng số tiền cho đơn hàng dựa trên số lượng và giá sản phẩm
                totalAmount += product.Price * itemDto.Quantity;

                orderItems.Add(new OrderItems
                {
                    OrderID = order.OrderID,
                    ProductID = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price // Giả sử bạn có thuộc tính Price trong OrderItems
                });

                // Giảm số lượng sản phẩm
                product.Quantity -= itemDto.Quantity;
            }

            // Thêm Order và OrderItems vào ngữ cảnh
            _context.Orders.Add(order);
            _context.orderitem.AddRange(orderItems);

            // Cập nhật tổng tiền cho đơn hàng
            order.TotalAmount = totalAmount;

            // Lưu tất cả các thay đổi chỉ một lần
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order created successfully.", OrderID = order.OrderID });
        }


        // GET: api/Orders/statistics
        [HttpGet("statistics")]
        public async Task<ActionResult> GetOrderStatistics()
        {
            // Xác định các mốc thời gian
            DateTime now = TimeHelper.NowVietnamTime();
            DateTime sevenDaysAgo = now.AddDays(-7);
            DateTime thirtyDaysAgo = now.AddDays(-30);
            DateTime ninetyDaysAgo = now.AddDays(-90);
            DateTime oneYearAgo = now.AddDays(-365);

            var ordersAll = await _context.Orders.CountAsync(); 

            // Đếm số lượng đơn hàng trong 7 ngày qua
            var ordersInLast7Days = await _context.Orders
                .Where(o => o.created_at >= sevenDaysAgo && o.created_at <= now)
                .CountAsync();

            // Đếm số lượng đơn hàng trong 30 ngày qua
            var ordersInLast30Days = await _context.Orders
                .Where(o => o.created_at >= thirtyDaysAgo && o.created_at <= now)
                .CountAsync();

            // Đếm số lượng đơn hàng trong 90 ngày qua
            var ordersInLast90Days = await _context.Orders
                .Where(o => o.created_at >= ninetyDaysAgo && o.created_at <= now)
                .CountAsync();

            // Đếm số lượng đơn hàng trong 365 ngày qua
            var ordersInLastYear = await _context.Orders
                .Where(o => o.created_at >= oneYearAgo && o.created_at <= now)
                .CountAsync();

            // Tính tổng doanh thu từ tất cả đơn hàng
            var totalRevenue = await _context.Orders.SumAsync(o => o.TotalAmount);


            // Trả về thông tin thống kê
            return Ok(new
            {
                sumOrders = new {
                    OrderAll = ordersAll,
                    OrdersInLast7Days = ordersInLast7Days,
                    OrdersInLast30Days = ordersInLast30Days,
                    OrdersInLast90Days = ordersInLast90Days,
                    OrdersInLastYear = ordersInLastYear
                },
                sumRevenueOrders = new { 
                    RevenueOrderAll = totalRevenue
                }
               
            });
        }

    }

}
