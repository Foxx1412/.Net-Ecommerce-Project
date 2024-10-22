using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Data;
using Project_1.Models;
using Project_1.NewFolder1;

namespace Project_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context) {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer) { 
            customer.created_at = TimeHelper.NowVietnamTime();
            customer.updated_at = TimeHelper.NowVietnamTime();

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();



            return Ok(new { success = true, message = "Add Customer Successfully", customer });
        }
    }
}
