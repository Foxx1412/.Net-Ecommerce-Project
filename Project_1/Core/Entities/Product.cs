using Project_1.Infrastructure.Data;
using System.Text.Json.Serialization;


namespace Project_1.Core.Entities
{
    public class Product
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Slug_name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public bool? status { get; set; }

        public int? ID_tag { get; set; }

        // Khóa ngoại liên kết với Category
        public int? ID_danhmuc { get; set; }
        [JsonIgnore]
        public Category? Category { get; set; }
        [JsonIgnore]
        public Tag? Tag { get; set; }
    }
}
