using Project_1.Core.Entities;
namespace Project_1.Core.Interfaces
{
    public interface IPaymentRepository
    {
        void AddPayment(Payment payment);
        Payment GetPaymentByTransactionId(string transactionId);

        List<Payment> GetAllPayments();

        bool DeletePaymentByTransactionId(string transactionId);
    }
}
