using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.Services;
using Project_1.Core.Entities;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomerController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers(int pageNumber = 1, int pageSize = 10)
        {
            var customer = await _customerService.GetAllCustomersAsync(pageNumber, pageSize);
            return Ok(customer);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }


        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer([FromBody] Customer customer)
        {
            await _customerService.AddCustomerAsync(customer);

            return Ok(new { success = true, message = "Add Customer Successfully", customer });
        }

        [HttpPut]
        public async Task<IActionResult> PutCustomer(Customer customer)
        {
            try
            {
                await _customerService.UpdateCustomerAsync(customer);
                return Ok("Cập nhật thông tin khách hàng thành công.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var isDeleted = await _customerService.DeleteCustomerAsync(id);

            if (!isDeleted)
            {
                return NotFound(new { Message = $"Khách hàng với ID {id} không tồn tại." });
            }

            return Ok(new { Message = "Khách hàng đã được xóa thành công." });
        }

    }
}
