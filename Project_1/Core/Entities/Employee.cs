using System.Runtime.CompilerServices;

namespace Project_1.Core.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        public int ID_Employee { get; set; }

        public String FullName { get; set; }

        public String Email { get; set; }

        public String Phone { get; set; }

        public decimal BaseSalary   { get; set; }

        public DateTime created_at { get; set; }

        public List<Attendance>? Attendances { get; set; }
        public List<Payroll>? Payrolls { get; set; }
    }
}
