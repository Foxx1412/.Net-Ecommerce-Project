namespace Project_1.Core.Entities
{
    public class Discount
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string DiscountType { get; set; } // 'Percent', 'Fixed', 'ProductSpecific', 'Coupon'
        public int DiscountValue { get; set; }
        public bool IsActive { get; set; }

        public bool IsExclusive { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? MinimumPurchase { get; set; } // Giá trị tối thiểu để áp dụng

        public int? ProductId { get; set; }
    }
}
