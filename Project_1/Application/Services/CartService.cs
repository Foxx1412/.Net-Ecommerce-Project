using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        public async Task AddToCartAsync(Cart cart)
        {
            var nowTime = DateTime.Now;

            // Lấy giỏ hàng hiện tại của khách hàng (nếu có)
            var existingCart = await _cartRepository.GetCartByCustomerIdAsync(cart.CustomerID);

            // Kiểm tra nếu CartItems không null
            if (cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new Exception("CartItems cannot be null or empty.");
            }

            // Lấy item từ giỏ hàng được truyền vào
            var item = cart.CartItems.FirstOrDefault();

            // Kiểm tra ProductId có tồn tại không
            var detailProduct = await _productRepository.GetProductByIdAsync(item.ProductId);
            var detailCustomer = await _customerRepository.getCustomerByIdAsync(cart.CustomerID);

            if (detailProduct == null || detailCustomer == null)
            {
                throw new Exception("Product or Customer does not exist.");
            }

            // Nếu chưa có giỏ hàng, tạo mới
            if (existingCart == null)
            {
                cart.created_at = nowTime;
                cart.TotalPrice = item.Quantity * detailProduct.Price; // Tính tổng giá dựa trên giá sản phẩm từ database
                cart.CartItems = new List<CartItems>(); // Khởi tạo danh sách CartItems

                // Thêm sản phẩm vào giỏ hàng mới
                cart.CartItems.Add(new CartItems
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = detailProduct.Price // Lấy giá từ sản phẩm
                });

                await _cartRepository.AddCartAsync(cart);
            }
            else
            {
                // Nếu đã có giỏ hàng, cập nhật giỏ hàng hiện tại
                // Tìm sản phẩm trong giỏ hàng đã có hay chưa
                var existingCartItem = existingCart.CartItems.FirstOrDefault(ci => ci.ProductId == item.ProductId);

                if (existingCartItem != null)
                {
                    // Nếu sản phẩm đã có trong giỏ hàng, tăng số lượng
                    existingCartItem.Quantity += item.Quantity;
                }
                else
                {
                    // Nếu sản phẩm chưa có trong giỏ hàng, thêm sản phẩm mới
                    existingCartItem = new CartItems
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = detailProduct.Price // Lấy giá từ sản phẩm
                    };
                    existingCart.CartItems.Add(existingCartItem);
                }

                // Cập nhật lại tổng giá trị của giỏ hàng
                existingCart.TotalPrice = existingCart.CartItems.Sum(i => i.Quantity * i.UnitPrice);

                await _cartRepository.UpdateCartAsync(existingCart);
            }
        }

        public async Task RemoveCartItemByIdAsync(int cartItemId)
        {
            await _cartRepository.DeleteCartItemByIdAsync(cartItemId);
        }

    }
}
