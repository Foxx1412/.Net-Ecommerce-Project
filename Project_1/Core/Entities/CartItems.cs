using System.ComponentModel.DataAnnotations;

namespace Project_1.Core.Entities
{
    public class CartItems
    {
        [Key]
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }

        public String? DiscountCodeSpecific { get; set; }  

        public Cart? Cart { get; set; }
    }
}
