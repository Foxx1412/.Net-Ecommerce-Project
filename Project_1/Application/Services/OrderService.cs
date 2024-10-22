using Project_1.Application.DTOs;
using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Repositories;
using Project_1.NewFolder1;
using System.Linq.Expressions;

namespace Project_1.Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly EmailService _emailService;
        private readonly ICustomerRepository _customerRepository;


        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, EmailService emailService, ICustomerRepository customerRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _emailService = emailService;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(int pageNumber, int pageSize)
        {
            return await _orderRepository.GetOrdersAsync(pageNumber, pageSize);
        }

        public async Task<Order> CreateOrderAsync(OrderDto orderDto)
        {
            // Generate OrderID
            var orderID = $"O-{DateTime.Now:yyyyMMddHHmmss}-{new Random().Next(1000, 9999)}";

            // Create new order
            var order = new Order
            {
                OrderID = orderID,
                CustomerID = orderDto.CustomerId,
                Status = "Pending",
                created_at = TimeHelper.NowVietnamTime(),
                updated_at = TimeHelper.NowVietnamTime(),
                OrderItems = new List<OrderItems>()
            };

            // Calculate total amount and assign order items
            decimal totalAmount = 0;
            foreach (var itemDto in orderDto.OrderItems)
            {
                // Get the product from the database
                var product = await _productRepository.GetProductByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product with ID {itemDto.ProductId} not found.");
                }

                // Ensure sufficient stock
                if (product.Quantity < itemDto.Quantity)
                {
                    throw new Exception($"Not enough stock for product ID {itemDto.ProductId}.");
                }

                var orderItem = new OrderItems
                {
                    OrderID = order.OrderID,
                    ProductID = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    Price = product.Price // Get the price from the product
                };
                totalAmount += product.Price * itemDto.Quantity;

                // Deduct the quantity from the product's stock
                product.Quantity -= itemDto.Quantity;

                order.OrderItems.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            // return await _orderRepository.CreateOrderAsync(order);
            var createdOrder = await _orderRepository.CreateOrderAsync(order);

            // Gửi email xác nhận đơn hàng
            try
            {
                var customer = await _customerRepository.getCustomerByIdAsync(orderDto.CustomerId);
                if (customer != null)
                {
                    // Tạo nội dung chi tiết đơn hàng cho email
                    var emailSubject = $"Order Confirmation - {order.OrderID}";
                    var emailBody = $@"
                        <h2>Thank you for your order!</h2>
                        <p>Your order ID is <strong>{order.OrderID}</strong></p>
                        <p><strong>Order Details:</strong></p>
                        <table border='1' cellpadding='5' cellspacing='0'>
                            <thead>
                                <tr>
                                    <th>Product Name</th>
                                    <th>Quantity</th>
                                    <th>Unit Price</th>
                                    <th>Total Price</th>
                                </tr>
                            </thead>
                            <tbody>";

                    // Thêm chi tiết sản phẩm vào nội dung email
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = await _productRepository.GetProductByIdAsync(orderItem.ProductID);
                        var productTotalPrice = orderItem.Price * orderItem.Quantity;
                        emailBody += $@"
                            <tr>
                                <td>{product.Name}</td>
                                <td>{orderItem.Quantity}</td>
                                <td>{orderItem.Price:C}</td>
                                <td>{productTotalPrice:C}</td>
                            </tr>";
                    }

                    emailBody += $@"
                        </tbody>
                    </table>
                    <p><strong>Total Amount: {totalAmount:C}</strong></p>
                    <p>We will notify you once your order is processed. Thank you for shopping with us!</p>";

                    // Gọi hàm gửi email
                    await _emailService.SendEmailAsync(customer.Email, emailSubject, emailBody);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }

            return createdOrder;
        }

        public async Task<int> GetOrderCountAsync()
        {
            return await _orderRepository.GetOrderCountAsync();
        }

        public async Task<int> GetOrderCountInTimeRangeAsync(DateTime from, DateTime to)
        {
            return await _orderRepository.GetOrderCountInTimeRangeAsync(from, to);
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _orderRepository.GetTotalRevenueAsync();
        }
    }
}
