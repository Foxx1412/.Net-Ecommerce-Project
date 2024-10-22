namespace Project_1.Models.DTOs
{
    public class OrderDto
    {
        public int CustomerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
