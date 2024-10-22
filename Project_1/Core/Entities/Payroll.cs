namespace Project_1.Core.Entities
{
    public class Payroll
    { 
        public int? Id { get; set; }
        public int ID_Employee { get; set; }

        public Employee Employee { get; set; }  

        public decimal GrossSalary { get; set; }

        public decimal NetSalary    { get; set; }

        public decimal Bonus { get; set; }

        public decimal Deductions { get; set; }

        public DateTime PayDate { get; set; }
    }
}
