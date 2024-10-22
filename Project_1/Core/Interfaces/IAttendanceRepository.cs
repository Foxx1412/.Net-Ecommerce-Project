using Project_1.Core.Entities;

namespace Project_1.Core.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<Attendance> GetAttendanceByIdAsync(int attendanceId);
        Task AddAttendanceAsync(Attendance attendance);
        Task UpdateAttendanceAsync(Attendance attendance);
        Task DeleteAttendanceAsync(int attendanceId);
        Task<IEnumerable<Attendance>> GetAttendancesByEmployeeIdAsync(int employeeId);

        Task<bool> CheckIfCheckedInTodayAsync(int employeeId);
    }
}
