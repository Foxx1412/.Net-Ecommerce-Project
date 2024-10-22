// Core/Interfaces/IProductRepository.cs
using Project_1.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Project_1.Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, int? minPrice, int? maxPrice, bool? status);
        Task<IEnumerable<Product>> GetAllProductsAsync(int pageNumber, int pageSize);
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
