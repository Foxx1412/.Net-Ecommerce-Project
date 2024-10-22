namespace Project_1.Core.Entities
{
    public class Attendance
    {
        public int Id { get; set; }

        public int ID_Employee { get; set; }

        public Employee? Employee { get; set; }

        public DateTime checkInTime { get; set; }

        public DateTime checkOutTime { get; set; }

        public decimal WorkHours { get; set; }

        public decimal WorkHoursOT  { get; set; }

        public DateTime WorkDate { get; set; }

        public void CalculateWorkHours()
        {
            // Tính giờ làm việc
            WorkHours = (decimal)(checkOutTime - checkInTime).TotalHours;

            // Giả sử giờ làm việc chính thức là 8 tiếng
            if (WorkHours > 8)
            {
                // WorkHoursOT = WorkHours - 8; // Tính giờ làm thêm
                WorkHours = 8; // Giới hạn giờ làm chính thức
            }
            /* else
            {
                WorkHoursOT = 0; // Không có giờ làm thêm
            }
            */
        }
    }
}
