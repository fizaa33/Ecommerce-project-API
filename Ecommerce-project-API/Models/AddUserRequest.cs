namespace Ecommerce_project_API.Models
{
    public class AddUserRequest
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
    public class LoginUserRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
