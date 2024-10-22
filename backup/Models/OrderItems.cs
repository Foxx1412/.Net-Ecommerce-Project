namespace Project_1.Models
{
    public class OrderItems
    {
        public int? Id { get; set; }

        public String OrderID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public int Price { get; set; }


        public Order Order { get; set; }

        public Product Product { get; set; }
    }
}
