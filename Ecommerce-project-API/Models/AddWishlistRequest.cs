namespace Ecommerce_project_API.Models
{
    public class AddWishlistRequest
    {
        public required int ProductId { get; set; }
        public required decimal Quantity { get; set; }
    }
}
