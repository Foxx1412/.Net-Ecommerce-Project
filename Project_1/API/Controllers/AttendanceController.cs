using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_1.Application.Services;
using Project_1.Core.Entities;
using Project_1.NewFolder1;

namespace Project_1.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AttendanceService _attendanceService;

        public AttendanceController(AttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpPost]
        public async Task<IActionResult> AddAttendance([FromBody] Attendance attendance)
        {
            var result = await _attendanceService.AddAttendanceAsync(attendance);
            return CreatedAtAction(nameof(GetAttendance), new { id = result.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendance(int id)
        {
            var attendance = await _attendanceService.GetAttendanceAsync(id);
            if (attendance == null)
                return NotFound();

            return Ok(attendance);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAttendance([FromBody] Attendance attendance)
        {
            await _attendanceService.UpdateAttendanceAsync(attendance);
            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(int id)
        {
            await _attendanceService.DeleteAttendanceAsync(id);
            return NoContent();
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetAttendancesByEmployeeId(int employeeId)
        {
            var attendances = await _attendanceService.GetAttendancesByEmployeeIdAsync(employeeId);
            return Ok(attendances);
        }
    }
}
