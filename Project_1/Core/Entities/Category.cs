namespace Project_1.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public bool status { get; set; }

        public DateTime? created_at { get; set; }

        public DateTime? updated_at { get; set; }   

        // Điều này cho phép liên kết với Product
        public ICollection<Product>? Product { get; set; }
    }
}
