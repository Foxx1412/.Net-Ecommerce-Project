using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize);

        Task<IEnumerable<Category>> FilterCategoriesAsync(int? id);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryByNameAsync(string categoryName);  // Thêm phương thức kiểm tra tên danh mục
        Task AddCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
    }
}
