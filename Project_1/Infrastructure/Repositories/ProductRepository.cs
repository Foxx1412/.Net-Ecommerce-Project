// Infrastructure/Repositories/ProductRepository.cs
using Project_1.Core.Interfaces;
using Project_1.Core.Entities;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Project_1.NewFolder1;

namespace Project_1.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(t => t.Tag)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Phương thức lọc sản phẩm theo nhiều tiêu chí
        public async Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, int? minPrice, int? maxPrice, bool? status)
        {
            // Khởi tạo truy vấn cơ bản lấy tất cả sản phẩm
            var query = _context.Products.AsQueryable();

            // Lọc theo Category (Brand)
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.ID_danhmuc == categoryId.Value);
            }

            // Lọc theo Price (Giá)
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Lọc theo Status
            if (status.HasValue)
            {
                query = query.Where(p => p.status == status.Value);
            }

            // Trả về danh sách sản phẩm đã lọc
            return await query.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
