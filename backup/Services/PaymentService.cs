using Microsoft.Extensions.Configuration;
using Project_1.Configurations;
using Project_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Project_1.Services
{
    public class PaymentService
    {
        private readonly VnpayConfig _vnpayConfig;

        public PaymentService(IConfiguration configuration)
        {
            _vnpayConfig = new VnpayConfig
            {
                vnp_TmnCode = configuration["VNPAY:TmnCode"],
                vnp_HashSecret = configuration["VNPAY:HashSecret"],
                vnp_Url = configuration["VNPAY:Url"],
                vnp_ReturnUrl = configuration["VNPAY:ReturnUrl"]
            };
        }

        public string CreatePaymentUrl(PaymentModel payment, string ipAddress)
        {
            // Tạo tham số cho giao dịch và sắp xếp theo thứ tự alphabet
            var vnpayData = new SortedDictionary<string, string>
    {
        { "vnp_Version", "2.1.0" },
        { "vnp_Command", "pay" },
        { "vnp_TmnCode", _vnpayConfig.vnp_TmnCode },  // Mã TmnCode của VNPay
        { "vnp_Amount", ((long)payment.Amount * 100).ToString() }, // Số tiền cần thanh toán (x100)
        { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") }, // Thời gian tạo giao dịch
        { "vnp_CurrCode", "VND" }, // Loại tiền tệ
        { "vnp_IpAddr", ipAddress }, // Địa chỉ IP của người dùng
        { "vnp_Locale", "vn" }, // Ngôn ngữ giao diện VNPay (vn hoặc en)
        { "vnp_OrderInfo", payment.OrderInfo }, // Thông tin giao dịch
        { "vnp_OrderType", "billpayment" }, // Loại giao dịch
        { "vnp_ReturnUrl", _vnpayConfig.vnp_ReturnUrl }, // URL trả về sau thanh toán
        { "vnp_TxnRef", DateTime.Now.Ticks.ToString() } // Mã giao dịch duy nhất
    };

            // Tạo chuỗi query string với thứ tự alphabet
            var queryString = string.Join("&", vnpayData.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));

            // Tạo chữ ký HMAC SHA512 từ chuỗi queryString (không bao gồm vnp_SecureHash)
            var secureHash = HmacSHA512(_vnpayConfig.vnp_HashSecret, queryString);

            // Tạo URL thanh toán đầy đủ bao gồm query string và vnp_SecureHash
            var paymentUrl = $"{_vnpayConfig.vnp_Url}?{queryString}&vnp_SecureHash={secureHash}";

            return paymentUrl;
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

    }
}
