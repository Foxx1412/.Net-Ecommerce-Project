using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.Services;
using Project_1.Core.Entities;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int pageNumber = 1, int pageSize = 10)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(pageNumber, pageSize);
            return Ok(categories);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterCategories([FromQuery] int? id)
        {
            var categories = await _categoryService.FilterCategoriesAsync(id);

            return Ok(categories);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }



        [HttpPost]
        public async Task<IActionResult> PostCategory(Category category)
        {
           
                if (category == null || string.IsNullOrEmpty(category.Name))
                {
                    return BadRequest(new { message = "Dữ liệu không hợp lệ" });
                }

                // Kiểm tra trùng tên danh mục
                var isAdded = await _categoryService.AddCategoryAsync(category);
                if (!isAdded)
                {
                    return Conflict(new { message = "Tên danh mục đã tồn tại" });
                }

                // Trả về thông báo thành công
                return Ok(new { success = true, message = "Category added successfully." });
           
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            await _categoryService.UpdateCategoryAsync(category);
            return Ok(new { success = true, message = "Category updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await _categoryService.DeleteCategoryAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { Message = $"Danh mục với ID {id} không tồn tại." });
            }

            return Ok(new { Message = "Danh mục đã được xóa thành công." });
        }

    }
}
