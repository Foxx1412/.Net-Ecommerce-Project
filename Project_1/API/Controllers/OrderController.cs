using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.DTOs;
using Project_1.Application.Services;
using Project_1.Core.Entities;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly EmailService _emailService;
        private readonly DiscountService _discountService;

        public OrderController(OrderService orderService, EmailService emailService, DiscountService discountService)
        {
            _orderService = orderService;
            _emailService = emailService;
            _discountService = discountService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders(int pageNumber = 1, int pageSize = 10)
        {
            var orders = await _orderService.GetOrdersAsync(pageNumber, pageSize);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderDto orderDto)
        {
            var order = await _orderService.CreateOrderAsync(orderDto);
            
            return Ok(new { message = "Order created successfully", order });
        }


        // 1. Endpoint để áp dụng mã giảm giá (Coupon)
        /* [HttpPost("apply-discount")]
        public async Task<ActionResult> ApplyDiscount(int orderId, string discountCode)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            try
            {
                var finalPrice = await _discountService.ApplyDiscountAsync(discountCode, order);
                order.TotalPrice = finalPrice;
                await _orderService.UpdateOrderAsync(order);

                return Ok(new { message = "Discount applied successfully", finalPrice });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. Endpoint để áp dụng giảm giá theo sản phẩm cụ thể
        [HttpPost("apply-product-discount")]
        public async Task<ActionResult> ApplyProductDiscount(int orderId)
        {
            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            try
            {
                var finalPrice = await _discountService.ApplyProductDiscount(order);
                await _orderService.UpdateOrderAsync(order);

                return Ok(new { message = "Product discounts applied successfully", finalPrice });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        */
        [HttpGet("statistics")]
        public async Task<ActionResult> GetOrderStatistics()
        {
            var now = DateTime.Now;
            var sevenDaysAgo = now.AddDays(-7);
            var thirtyDaysAgo = now.AddDays(-30);
            var ninetyDaysAgo = now.AddDays(-90);
            var oneYearAgo = now.AddYears(-1);

            var totalOrders = await _orderService.GetOrderCountAsync();
            var ordersLast7Days = await _orderService.GetOrderCountInTimeRangeAsync(sevenDaysAgo, now);
            var ordersLast30Days = await _orderService.GetOrderCountInTimeRangeAsync(thirtyDaysAgo, now);
            var ordersLast90Days = await _orderService.GetOrderCountInTimeRangeAsync(ninetyDaysAgo, now);
            var ordersLastYear = await _orderService.GetOrderCountInTimeRangeAsync(oneYearAgo, now);
            var totalRevenue = await _orderService.GetTotalRevenueAsync();

            return Ok(new
            {
                totalOrders,
                ordersLast7Days,
                ordersLast30Days,
                ordersLast90Days,
                ordersLastYear,
                totalRevenue
            });
        }
    }
}
