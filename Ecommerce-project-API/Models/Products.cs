namespace Ecommerce_project_API.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public decimal Quantity { get; set; }
        public int CategoryId { get; set; }  // Foreign Key
        public string ImagePath { get; set; }
        public int SearchCount { get; set; } = 0;
        public int SoldQuantity { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
