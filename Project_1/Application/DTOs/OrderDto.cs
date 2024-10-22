namespace Project_1.Application.DTOs
{
    public class OrderDto
    {
        public int CustomerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }
}
