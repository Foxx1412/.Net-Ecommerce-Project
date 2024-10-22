namespace Project_1.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Điều này cho phép liên kết với Product
        public ICollection<Product> Product { get; set; }
    }
}
