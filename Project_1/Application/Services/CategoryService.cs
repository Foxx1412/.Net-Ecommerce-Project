using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Repositories;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class CategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            return await _categoryRepository.GetAllCategoriesAsync(pageNumber, pageSize);
        }

        public async Task<IEnumerable<Category>> FilterCategoriesAsync(int? id)
        {
            return await _categoryRepository.FilterCategoriesAsync(id);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetCategoryByIdAsync(id);
        }

        public async Task<bool> AddCategoryAsync(Category category)
        {
            var checkSameName = await _categoryRepository.GetCategoryByNameAsync(category.Name);
            if (checkSameName != null) {
                return false;
            }
            var timeNow = TimeHelper.NowVietnamTime();
            category.created_at = timeNow;
            await _categoryRepository.AddCategoryAsync(category);
            return true;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await _categoryRepository.UpdateCategoryAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            return true;
        }
    }
}
