using Project_1.Core.Interfaces;
using Project_1.Core.Entities;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Project_1.Infrastructure.Repositories

{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            return await _context.Categories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> FilterCategoriesAsync(int? id) {
            var query = _context.Categories.AsQueryable();

            if (id.HasValue) {
                query = query.Where(c => c.Id == id.Value);
            }
            
            return await query.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == categoryName.ToLower());
        }

        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
           
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
