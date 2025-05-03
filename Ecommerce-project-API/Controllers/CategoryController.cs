using Ecommerce_project_API.Models;
using Ecommerce_project_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_project_API.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AuthService _authService;
        private myDbContext _context;

        public CategoryController(myDbContext context, AuthService authService)
        {

            _context = context;
            _authService = authService;

        }

        [HttpGet]
        public async Task<ActionResult> GetAllCategories()
        {
            var data = await _context.Category.ToListAsync();
            var response = new ApiResponse<List<Categories>>
            {
                Message = "Categories fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = data
            };
            return Ok(response);
        }
        [Authorize]
        [HttpPost]

        public async Task<ActionResult> AddCategory([FromForm]  AddCategoryRequest request)
        {
            if(request.File == null || request.File.Length == 0)
            {
                return BadRequest("No file Uploaded");
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

            var category = new Categories
            {
                Name = request.Name,
                ImagePath = ImagePath
            };


            _context.Category.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Message = "category added successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });

        }

        [Authorize]
        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateCategory([FromForm] AddCategoryRequest request, int Id)
        {
            var category = await _context.Category.FindAsync(Id);

            if (category == null)
            {
           
              
                    return NotFound(new ApiResponse<object>
                    {
                        Message = "Category Not Found",
                        Status = 404,
                        IsSuccess = false,
                        Data = null
                    });
                
            }

            // Update name
            category.Name = request.Name;

            // If a new file is uploaded, update the image
            if (request.File != null && request.File.Length > 0)
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", request.File.FileName);
                var directoryPath = Path.GetDirectoryName(imagePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (var fileStream = new FileStream(imagePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                category.ImagePath = imagePath;
            }

            _context.Category.Update(category);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Message = "Category Updated successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });
        }

       
        [HttpGet("{Id}")]

        public async Task<ActionResult> GetCategoryById(int Id)
        {
            var category = await _context.Category.FindAsync(Id);
            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Category Not Found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }
            var response = new ApiResponse<object>
            {
                Message = "Category fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = category
            };
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteCategory(int Id)
        {

            var category = await _context.Category.FindAsync(Id);

            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Category Not Found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }

            _context.Category.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object>
            {
                Message = "Category Deleted successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });
        }



    }
}
