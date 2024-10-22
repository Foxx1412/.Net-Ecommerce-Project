using Project_1.Core.Entities;
using Project_1.Core.Interfaces;
using Project_1.NewFolder1;

namespace Project_1.Application.Services
{
    public class PayrollService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollRepository _payrollRepository;

        public PayrollService(IEmployeeRepository employeeRepository, IPayrollRepository payrollRepository)
        {
            _employeeRepository = employeeRepository;
            _payrollRepository = payrollRepository;
        }

        public async Task<decimal> CalculatePayrollAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);

            if (employee == null)
            {
                throw new Exception("Nhân viên không tồn tại.");
            }

            // Kiểm tra nếu danh sách Attendance của nhân viên không null
            if (employee.Attendances == null)
            {
                throw new Exception("Danh sách chấm công không thể là null.");
            }

            // Tính tháng và năm hiện tại
            var currentMonth = TimeHelper.NowVietnamTime().Month;
            var currentYear = TimeHelper.NowVietnamTime().Year;

            // Tính tổng số giờ làm việc cho tháng hiện tại (không tính Chủ nhật)
            var totalWorkHours = employee.Attendances
                .Where(a => a.ID_Employee == employeeId && a.WorkDate.Month == currentMonth && a.WorkDate.Year == currentYear && a.WorkDate.DayOfWeek != DayOfWeek.Sunday)
                .Sum(a => a.WorkHours);

            // Giả sử một tháng có 208 giờ làm việc tiêu chuẩn (8 giờ/ngày * 26 ngày làm việc)
            var standardMonthlyWorkHours = 8 * 26; // Hoặc có thể điều chỉnh số giờ tiêu chuẩn mỗi tháng nếu cần

            // Lương cơ bản theo tháng đã lưu trong BaseSalary
            var baseSalary = employee.BaseSalary; // Lương tháng

            // Tính lương trên mỗi giờ
            var hourlyRate = baseSalary / standardMonthlyWorkHours;

            // Tính tổng lương dựa trên tổng số giờ làm thực tế trong tháng
            var grossSalary = hourlyRate * totalWorkHours;

            // Định nghĩa tiền thưởng (Bonus) và khấu trừ (Deductions)
            var bonus = 0; // Tiền thưởng cố định cho ví dụ
            var deductions = 0; // Không có khấu trừ

            // Tính lương thực nhận (Net Salary)
            var netSalary = grossSalary + bonus - deductions;

            // Tạo bản ghi Payroll cho tháng hiện tại
            var payroll = new Payroll
            {
                ID_Employee = employeeId,
                GrossSalary = grossSalary,
                NetSalary = netSalary,
                Bonus = bonus,
                Deductions = deductions,
                PayDate = TimeHelper.NowVietnamTime(), // Ngày trả lương (hiện tại)
            };

            // Lưu bản ghi Payroll
            await _payrollRepository.AddPayrollAsync(payroll);

            return totalWorkHours;
        }
    }
}
