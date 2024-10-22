namespace Project_1.Core.Entities
{
    public class Customer
    {
        public int? Id { get; set; }

        public int ID_User { get; set; }
        public String FullName { get; set; }

        public String Email { get; set; }

        public String Phone { get; set; } 

        public String Address { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }

    }
}
