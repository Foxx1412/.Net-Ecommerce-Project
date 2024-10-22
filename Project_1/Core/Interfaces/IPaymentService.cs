using Project_1.Application.DTOs;
using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IPaymentService
    {
        string CreatePaymentUrl(Payment payment, string ipAddress);
        void ProcessPayment(Payment payment);
        string HmacSHA512(string key, string data);
        Payment GetPaymentByTransactionId(string transactionId);
    }
}
