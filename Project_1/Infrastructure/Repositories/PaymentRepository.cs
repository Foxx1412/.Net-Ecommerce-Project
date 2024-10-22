using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;
using Project_1.NewFolder1;

namespace Project_1.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Payment> GetAllPayments()
        {
            return _context.Payments.OrderByDescending(x => x.created_at).ToList();
        }

        public Payment GetPaymentByTransactionId(string transactionId)
        {
            return _context.Payments.FirstOrDefault(p => p.TransactionId == transactionId);
        }

        public void AddPayment(Payment payment)
        {
            _context.Payments.Add(payment);
            _context.SaveChanges();
        }

        // Phương thức xóa một giao dịch theo TransactionId
        public bool DeletePaymentByTransactionId(string transactionId)
        {
            var payment = _context.Payments.FirstOrDefault(p => p.TransactionId == transactionId);
            if (payment != null)
            {
                _context.Payments.Remove(payment);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
