using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project_1.Application.DTOs;
using Project_1.Application.Services;
using Project_1.Core.Entities;
using Project_1.Core.Interfaces;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentController(PaymentService paymentService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] Payment payment)
        {
            // Kiểm tra dữ liệu vào
            if (payment == null || string.IsNullOrEmpty(payment.OrderInfo) || payment.Amount <= 0)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            // Lấy IP từ HttpContext
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

            // Chuyển đổi IPv6 sang IPv4 nếu cần thiết
            if (ip == "::1")
            {
                ip = "127.0.0.1";  // localhost IPv4
            }
            // Tạo URL thanh toán
            var paymentUrl = _paymentService.CreatePaymentUrl(payment, ip);

            return Ok(new { paymentUrl });
        }

        // API endpoint để lấy tất cả các giao dịch, sắp xếp từ mới nhất đến cũ nhất
        [HttpGet("  ")]
        public IActionResult GetAllPayments()
        {
            var payments = _paymentService.GetAllPayments();

            if (payments == null || payments.Count == 0)
            {
                return NotFound(new { message = "Không có giao dịch nào" });
            }

            return Ok(payments);
        }

        [HttpPost("add-payment")]
        public IActionResult AddPayment([FromBody] Payment payment)
        {
            if (payment == null || string.IsNullOrEmpty(payment.OrderInfo) || payment.Amount <= 0)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ" });
            }

            // Thêm giao dịch thanh toán mới
            _paymentService.AddPayment(payment);

            return Ok(new { message = "Thêm giao dịch thành công", payment });
        }

        // API endpoint để xóa một giao dịch theo TransactionId
        [HttpDelete("delete-payment/{transactionId}")]
        public IActionResult DeletePayment(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                return BadRequest(new { message = "TransactionId không được để trống" });
            }

            var result = _paymentService.DeletePayment(transactionId);

            if (result)
            {
                return Ok(new { message = "Xóa giao dịch thành công" });
            }
            else
            {
                return NotFound(new { message = "Không tìm thấy giao dịch" });
            }
        }

        [HttpGet("vnpay_return")]
        public IActionResult VnpayReturn()
        {
            // Lấy HashSecret từ appsettings.json
            var vnp_HashSecret = _configuration["VNPAY:HashSecret"];
            if (string.IsNullOrEmpty(vnp_HashSecret))
            {
                return BadRequest(new { Message = "Missing hash secret configuration" });
            }

            // Lấy các tham số trả về từ VNPAY
            var vnpayData = Request.Query
                .Where(kvp => kvp.Key.StartsWith("vnp_"))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            if (!vnpayData.ContainsKey("vnp_SecureHash"))
            {
                return BadRequest(new { Message = "Missing vnp_SecureHash in response" });
            }

            // Sắp xếp các tham số và bỏ qua vnp_SecureHash
            var sortedParams = vnpayData
                .Where(kvp => kvp.Key != "vnp_SecureHash")
                .OrderBy(kvp => kvp.Key)
                .Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"); // Escape dữ liệu tham số

            var hashData = string.Join("&", sortedParams);

            // Tạo lại chữ ký
            var secureHash = _paymentService.HmacSHA512(vnp_HashSecret, hashData);

            // So sánh chữ ký theo cách an toàn (bỏ qua phân biệt hoa thường)
            if (secureHash.Equals(vnpayData["vnp_SecureHash"], StringComparison.OrdinalIgnoreCase))
            {
                if (vnpayData.ContainsKey("vnp_ResponseCode") && vnpayData["vnp_ResponseCode"] == "00")
                {
                    return Ok(new { Message = "Transaction success", Data = vnpayData });
                }
                else
                {
                    return BadRequest(new { Message = "Transaction failed", Data = vnpayData });
                }
            }
            else
            {
                return BadRequest(new { Message = "Invalid signature", Data = vnpayData });
            }
        }

        [HttpGet("{transactionId}")]
        public IActionResult GetPaymentDetails(string transactionId)
        {
            // Kiểm tra transactionId có tồn tại không
            if (string.IsNullOrEmpty(transactionId))
            {
                return BadRequest(new { message = "Transaction ID không được để trống" });
            }

            // Gọi service để lấy thông tin thanh toán
            var payment = _paymentService.GetPaymentByTransactionId(transactionId);

            if (payment == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin thanh toán cho transaction ID này" });
            }

            return Ok(payment);
        }

    }
}
