namespace Ecommerce_project_API.Models
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public bool IsSuccess { get; set; }
        public List<string>? Errors { get; set; }
        public T? Data { get; set; }

        public ApiResponse()
        {
            Errors = new List<string>();
        }
    }

}
