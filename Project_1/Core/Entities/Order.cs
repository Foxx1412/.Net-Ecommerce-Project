namespace Project_1.Core.Entities
{
    public class Order
    {
        public String OrderID { get; set; }

        public int CustomerID { get; set; }

        public String? DiscountCode { get; set; }

        public Decimal TotalAmount { get; set; }

        public String Status { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }


        public Customer Customer { get; set; }

        public ICollection<OrderItems>? OrderItems { get; set; }
    }
}
