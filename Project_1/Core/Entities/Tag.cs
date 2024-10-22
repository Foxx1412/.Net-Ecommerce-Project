namespace Project_1.Core.Entities
{
    public class Tag
    {
        public int? Id { get; set; }

        public String Name { get; set; }

        // Điều này cho phép liên kết với Product
        public ICollection<Product>? Product { get; set; }
    }
}
