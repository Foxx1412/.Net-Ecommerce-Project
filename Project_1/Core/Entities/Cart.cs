using Amazon.S3.Model;
using System.ComponentModel.DataAnnotations;

namespace Project_1.Core.Entities
{
    public class Cart
    {
        public int Id { get; set; } 
        public int CustomerID { get; set; }
        public string? DiscountCode { get; set; }
        public int TotalPrice { get; set; }
        public DateTime created_at { get; set; }
        // Navigation property
        public List<CartItems> CartItems { get; set; }
    }
}
