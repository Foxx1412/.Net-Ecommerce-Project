// UI/Controllers/ProductController.cs
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.Services;
using Project_1.Core.Entities;

namespace Project_1.UI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly EmailService _emailService;

        public ProductController(ProductService productService, EmailService emailService)
        {
            _productService = productService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int pageNumber = 1, int pageSize = 10)
        {
            var products = await _productService.GetAllProductsAsync(pageNumber, pageSize);
            return Ok( new { products = products });
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // API để lọc sản phẩm theo danh mục (brand), giá và trạng thái
        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts([FromQuery] int? categoryId, [FromQuery] int? minPrice, [FromQuery] int? maxPrice, [FromQuery] bool? status)
        {
            var products = await _productService.FilterProductsAsync(categoryId, minPrice, maxPrice, status);

            return Ok(products);
        }


        [HttpPost]
        public async Task<IActionResult> PostProduct(Product product)
        {
            await _productService.AddProductAsync(product);
            // Gửi email thông báo về sản phẩm mới (nếu cần)
            string subject = "Sản phẩm mới đã được thêm thành công!";
            string body = $"Sản phẩm {product.Name} đã được thêm vào với giá {product.Price} và Slug_name {product.Slug_name}.";
            await _emailService.SendEmailAsync("phucn1435@gmail.com", subject, body);
            return Ok(new { success = true, message = "Product added successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            await _productService.UpdateProductAsync(product);
            return Ok(new { success = true, message = "Product updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var isDeleted = await _productService.DeleteProductAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { Message = $"Sản phẩm với ID {id} không tồn tại." });
            }

            return Ok(new { Message = "Sản phẩm đã được xóa thành công." });
        }
        
    }
}
