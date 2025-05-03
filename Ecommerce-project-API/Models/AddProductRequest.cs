namespace Ecommerce_project_API.Models
{
    public class AddProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public int CategoryId { get; set; }  

        public IFormFile File { get; set; }
    }
}
