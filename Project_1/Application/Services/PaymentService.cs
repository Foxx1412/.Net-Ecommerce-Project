using Project_1.Configurations;
using System.Security.Cryptography;
using System.Text;
using Project_1.Application.DTOs;
using Project_1.Core.Interfaces;
using Project_1.Core.Entities;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class PaymentService
    {
        //private readonly VnpayConfig _vnpayConfig;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IConfiguration _config;
        public PaymentService(IPaymentRepository paymentRepository,IConfiguration configuration)
        {
            _paymentRepository = paymentRepository;
            _config = configuration;

        }

        public string CreatePaymentUrl(Payment payment, string ipAddress)
        {
            // Tạo tham số cho giao dịch và sắp xếp theo thứ tự alphabet
            var vnpayData = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _config["VNPAY:TmnCode"] },  // Mã TmnCode của VNPay
                { "vnp_Amount", ((long)payment.Amount * 100).ToString() }, // Số tiền cần thanh toán (x100)
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }, // Thời gian tạo giao dịch
                { "vnp_CurrCode", "VND" }, // Loại tiền tệ
                { "vnp_IpAddr", ipAddress }, // Địa chỉ IP của người dùng
                { "vnp_Locale", "vn" }, // Ngôn ngữ giao diện VNPay (vn hoặc en)
                { "vnp_OrderInfo", payment.OrderInfo }, // Thông tin giao dịch
                { "vnp_OrderType", "billpayment" }, // Loại giao dịch
                { "vnp_ReturnUrl",  _config["VNPAY:ReturnUrl"] }, // URL trả về sau thanh toán
                { "vnp_TxnRef", DateTime.Now.Ticks.ToString() } // Mã giao dịch duy nhất
            };

            // Tạo chuỗi query string với thứ tự alphabet
            var queryString = string.Join("&", vnpayData.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            // Tạo chữ ký HMAC SHA512 từ chuỗi queryString (không bao gồm vnp_SecureHash)
            var secureHash = HmacSHA512(_config["VNPAY:HashSecret"], queryString);

            // Tạo URL thanh toán đầy đủ bao gồm query string và vnp_SecureHash
            var paymentUrl = $"{_config["VNPAY:Url"]}?{queryString}&vnp_SecureHash={secureHash}";

            return paymentUrl;
        }

        public List<Payment> GetAllPayments()
        { 
            return _paymentRepository.GetAllPayments();
        }

        // Phương thức để thêm giao dịch thanh toán mới
        public void AddPayment(Payment payment)
        {
            // Có thể thêm logic kiểm tra/validate ở đây
            var timeNow = TimeHelper.NowVietnamTime();
            payment.created_at = timeNow;// Gán thời gian tạo giao dịch hiện tại
            payment.Status = "pending";  // Gán trạng thái ban đầu của giao dịch

            _paymentRepository.AddPayment(payment);
        }



        public string HmacSHA512(string key, string data)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (string.IsNullOrEmpty(data)) throw new ArgumentNullException(nameof(data));

            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public Payment GetPaymentByTransactionId(string transactionId)
        {
            return _paymentRepository.GetPaymentByTransactionId(transactionId);
        }

        // Phương thức xóa một giao dịch theo TransactionId
        public bool DeletePayment(string transactionId)
        {
            return _paymentRepository.DeletePaymentByTransactionId(transactionId);
        }

    }
}

