using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class AttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository;

        public AttendanceService(IAttendanceRepository attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Attendance> AddAttendanceAsync(Attendance attendance)
        {
            // Kiểm tra xem nhân viên đã check-in hôm nay chưa
            bool hasCheckedIn = await _attendanceRepository.CheckIfCheckedInTodayAsync(attendance.ID_Employee);
            if (hasCheckedIn)
            {
                throw new Exception("Nhân viên đã check-in hôm nay.");
            }

            // Cập nhật thời gian check-in và ngày làm việc
            attendance.checkInTime = TimeHelper.NowVietnamTime();
            attendance.WorkDate = TimeHelper.NowVietnamTime().Date; // Lưu ý chỉ lấy ngày

            // Thêm bản ghi Attendance vào cơ sở dữ liệu
            await _attendanceRepository.AddAttendanceAsync(attendance);
            return attendance;
        }


        public async Task<Attendance> GetAttendanceAsync(int attendanceId)
        {
            return await _attendanceRepository.GetAttendanceByIdAsync(attendanceId);
        }

        public async Task<Attendance> UpdateAttendanceAsync(Attendance attendance)
        {
            // Lấy Attendance dựa trên ID
            var existingAttendance = await _attendanceRepository.GetAttendanceByIdAsync(attendance.ID_Employee);
            if (existingAttendance == null)
            {
                throw new Exception("Attendance không tồn tại.");
            }

            // Cập nhật thời gian check-out
            if (attendance.checkOutTime != default)
            {
                existingAttendance.checkOutTime = attendance.checkOutTime;

                // Tính toán giờ làm việc
                if (existingAttendance.checkOutTime > existingAttendance.checkInTime)
                {
                    existingAttendance.WorkHours = (decimal)(existingAttendance.checkOutTime - existingAttendance.checkInTime).TotalHours;
                    if (existingAttendance.WorkHours >= 8)
                    {
                        existingAttendance.WorkHours = 8;
                    }
                   
                }
                else
                {
                    throw new Exception("Thời gian check-out phải lớn hơn thời gian check-in.");
                }
            }

            // Cập nhật giờ làm thêm (OT)
            if (attendance.WorkHoursOT > 0)
            {
                existingAttendance.WorkHoursOT = attendance.WorkHoursOT;
            }

            // Cập nhật Attendance trong cơ sở dữ liệu
            await _attendanceRepository.UpdateAttendanceAsync(existingAttendance);
            return existingAttendance;
        }


        public async Task DeleteAttendanceAsync(int attendanceId)
        {
            await _attendanceRepository.DeleteAttendanceAsync(attendanceId);
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByEmployeeIdAsync(int employeeId)
        {
            return await _attendanceRepository.GetAttendancesByEmployeeIdAsync(employeeId);
        }
    }
}
