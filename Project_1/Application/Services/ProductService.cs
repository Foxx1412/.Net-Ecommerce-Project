// Application/Services/ProductService.cs
using Project_1.Core.Entities;
using Project_1.Core.Interfaces;


namespace Project_1.Application.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            return await _productRepository.GetAllProductsAsync(pageNumber, pageSize);
        }

        // Phương thức lọc sản phẩm
        public async Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, int? minPrice, int? maxPrice, bool? status)
        {
            return await _productRepository.FilterProductsAsync(categoryId, minPrice, maxPrice, status);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null) {
                return false;
            }

            await _productRepository.DeleteProductAsync(id);
            return true;
        }
    }
}
