using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.Services;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollController : ControllerBase
    {
        private readonly PayrollService _payrollService;

        public PayrollController(PayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [HttpPost("calculate/{employeeId}")]
        public async Task<IActionResult> CalculatePayroll(int employeeId)
        {
            try
            {
                var payroll = await _payrollService.CalculatePayrollAsync(employeeId);
                return Ok(payroll);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
