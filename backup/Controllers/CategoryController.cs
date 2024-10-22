using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_1.Data;
using Project_1.Models;

namespace Project_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;

        public CategoryController(AppDbContext context) { 
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories(int pageNumber = 1, int pageSize = 10)
        {
            // Lấy danh sách Category từ database
            var categories = await _context.Categories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCategories = await _context.Categories.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCategories / (double)pageSize);

            var result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalCategories = totalCategories,
                Categories = categories
            };

            // Trả về một object ẩn danh bọc danh sách Category với trường "Category"
            return Ok(new { Category = result });
        }
    }
}
