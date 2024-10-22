using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Project_1.Infrastructure.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance> GetAttendanceByIdAsync(int attendanceId)
        {
            return await _context.Attendance.FindAsync(attendanceId);
        }

        public async Task AddAttendanceAsync(Attendance attendance)
        {
            await _context.Attendance.AddAsync(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAttendanceAsync(Attendance attendance)
        {
            _context.Attendance.Update(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAttendanceAsync(int attendanceId)
        {
            var attendance = await GetAttendanceByIdAsync(attendanceId);
            if (attendance != null)
            {
                _context.Attendance.Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByEmployeeIdAsync(int employeeId)
        {
            return await _context.Attendance
                .Where(a => a.ID_Employee == employeeId)
                .ToListAsync();
        }

        public async Task<bool> CheckIfCheckedInTodayAsync(int employeeId)
        {
            var today = DateTime.Now.Date;
            return await _context.Attendance.AnyAsync(a =>
                a.ID_Employee == employeeId &&
                a.WorkDate.Date == today);
        }
    }
}
