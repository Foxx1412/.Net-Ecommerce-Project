using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Repositories;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class DiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ICartRepository _cartRepository;

        public DiscountService(IDiscountRepository discountRepository, ICartRepository cartRepository)
        {
            _discountRepository = discountRepository;
            _cartRepository = cartRepository;
        }

        public async Task<int> ApplyDiscountAsync(string discountCode, Cart cart)
        {
            var discount = await _discountRepository.GetDiscountByCodeAsync(discountCode);
            if (discount == null)
            {
                throw new Exception("Mã giảm giá không hợp lệ.");
            }

            // Lấy danh sách mã giảm giá hiện tại từ giỏ hàng
            var currentDiscountCodes = cart.DiscountCode?.Split(',').Select(dc => dc.Trim()).ToList() ?? new List<string>();

            // Kiểm tra nếu mã giảm giá đã tồn tại trong giỏ hàng
            if (currentDiscountCodes.Contains(discountCode))
            {
                throw new Exception("Mã giảm giá đã được áp dụng trước đó.");
            }

            // Áp dụng giảm giá dựa trên loại mã giảm giá
            int finalPrice = cart.TotalPrice;

            switch (discount.DiscountType)
            {
                case "Percent":
                    if (cart.TotalPrice < discount.MinimumPurchase)
                    {
                        throw new Exception("Not available");
                    }

                    finalPrice = cart.TotalPrice - (cart.TotalPrice * discount.DiscountValue / 100);
                    break;

                case "Fixed":
                    if (!discount.IsExclusive)
                    {
                        throw new Exception("This discount code is not applicable with other codes");
                    }
                    finalPrice = cart.TotalPrice - discount.DiscountValue;
                    break;

                case "ProductSpecific":
                    var applicableItem = cart.CartItems.FirstOrDefault(item => item.ProductId == discount.ProductId);

                    if (applicableItem == null)
                    {
                        throw new Exception("This cart doesn't have the specified product");
                    }

                    // Kiểm tra nếu sản phẩm đã có mã giảm giá trong CartItem
                    var currentItemDiscountCodes = applicableItem.DiscountCodeSpecific?.Split(',').Select(dc => dc.Trim()).ToList() ?? new List<string>();
                    if (currentItemDiscountCodes.Contains(discountCode))
                    {
                        throw new Exception("Mã giảm giá đã được áp dụng cho sản phẩm này.");
                    }

                    // Tính toán giá trị giảm giá
                    var discountAmount = applicableItem.UnitPrice * (discount.DiscountValue / 100); // Giảm giá tính theo phần trăm
                    applicableItem.UnitPrice -= discountAmount; // Áp dụng giảm giá vào giá sản phẩm

                    // Cập nhật mã giảm giá vào CartItem
                    if (string.IsNullOrEmpty(applicableItem.DiscountCodeSpecific))
                    {
                        applicableItem.DiscountCodeSpecific = discountCode;
                    }
                    else
                    {
                        applicableItem.DiscountCodeSpecific += $",{discountCode}";
                    }

                    // Cập nhật lại tổng giá trị giỏ hàng
                    finalPrice = cart.CartItems.Sum(item => item.Quantity * item.UnitPrice);
                    break;

                default:
                    throw new Exception("Loại giảm giá không hợp lệ.");
            }

            // Đảm bảo giá trị không âm
            if (finalPrice < 0)
            {
                finalPrice = 0;
            }

            return finalPrice;
        }




    }
}
