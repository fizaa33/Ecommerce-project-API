namespace Ecommerce_project_API.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }

        public decimal Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public Products Product { get; set; }

        public UserModel User { get; set; }  


    }
}
