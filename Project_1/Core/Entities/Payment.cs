namespace Project_1.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string OrderInfo { get; set; }
        public decimal Amount { get; set; }

        public string Method { get; set; }
        public string Status { get; set; } // Thêm trạng thái thanh toán nếu cần

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }
        
    }
}
