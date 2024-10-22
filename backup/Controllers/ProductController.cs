using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project_1.Data;
using Project_1.Models;
using Newtonsoft.Json;
using Project_1.NewFolder1;


namespace Project_1.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        public ProductController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        /* public static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };
        */


        // GET: api/Product
        /* 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(int pageNumber = 1,int pageSize = 10)
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalProducts = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var finalProduct = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Price,
                Category = new
                {
                    p.Category.Id,
                    p.Category.Name
                }
            }).ToList();

            var result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalProducts = totalProducts,
                Products = finalProduct
            };

            return Ok(new { products = result });
        }

        // GET: api/Product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProductById(int id)
        {
            // Tìm kiếm sản phẩm theo ID trong cơ sở dữ liệu
            var product = await _context.Products
                .Include(p => p.Category) // Nếu muốn bao gồm cả thông tin về Category
                .FirstOrDefaultAsync(p => p.Id == id);

            // Kiểm tra nếu sản phẩm không tồn tại
            if (product == null)
            {
                return NotFound(new { message = $"Sản phẩm với ID = {id} không tồn tại." });
            }

            // Trả về sản phẩm tìm thấy với thông tin về category
            var result = new
            {
                product.Id,
                product.Name,
                product.Price,
                product.Slug_name,
                Category = new
                {
                    product.Category.Id,
                    product.Category.Name
                }
            };

            return Ok(result);
        }


        // POST: api/Product
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            // Thiết lập giá trị Created_at và Updated_at theo múi giờ Việt Nam
           
            product.created_at = TimeHelper.NowVietnamTime();
            product.updated_at = TimeHelper.NowVietnamTime();

            // Thêm sản phẩm vào database
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Gửi email thông báo về sản phẩm mới (nếu cần)
            string subject = "Sản phẩm mới đã được thêm thành công!";
            string body = $"Sản phẩm {product.Name} đã được thêm vào với giá {product.Price} và Slug_name {product.Slug_name}.";
            // await _emailService.SendEmailAsync("phucn1435@gmail.com", subject, body);

            // Trả về thông tin sản phẩm vừa được tạo, với đường dẫn đến nó
            return Ok(new { success = true, message = "Product added successfully.", product });
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.Combine("wwwroot/uploads", file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the file path or name to the client
                return Ok(new { filePath = Path.Combine("/uploads", file.FileName) });
            }

            return BadRequest("No file received.");
        }


        // DELETE: api/Product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Product deleted successfully." }); 
        }

        // PUT: api/Product/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(); // Kiểm tra nếu ID không khớp
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound(new { success = false, message = "Product not found." });
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { success = true, message = "Product updated successfully." });
        }

        // Hàm kiểm tra sản phẩm có tồn tại trong DB hay không
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        */

    }
}

