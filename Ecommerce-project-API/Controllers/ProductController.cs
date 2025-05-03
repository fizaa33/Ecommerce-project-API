using Ecommerce_project_API.Models;
using Ecommerce_project_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_project_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly AuthService _authService;
        private myDbContext _context;

        public ProductController(myDbContext context, AuthService authService)
        {

            _context = context;
            _authService = authService;

        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var data = await _context.Products.ToListAsync();
            var response = new ApiResponse<List<Products>>
            {
                Message = "Products fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = data
            };
            return Ok(response);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult> GetProductById(int Id)
        {
            var product = await _context.Products.FindAsync(Id);

            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Product Not Found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }
            return Ok(new ApiResponse<object>
            {
                Message = "Product Get successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = product
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromForm] AddProductRequest request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest("No file Uploaded");
            }

            var categoryExists = await _context.Category.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = "Invalid category ID.",
                    Status = 400,
                    IsSuccess = false,
                 
                });
            }

            var ImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", request.File.FileName);

            var directoryPath = Path.GetDirectoryName(ImagePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (var fileStream = new FileStream(ImagePath, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream);
            }

            var product = new Products
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Quantity = request.Quantity,
                CategoryId = request.CategoryId,
                ImagePath = ImagePath,
                    CreatedAt = DateTime.UtcNow

            };


            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Message = "Product added successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });

        }
    }
}
