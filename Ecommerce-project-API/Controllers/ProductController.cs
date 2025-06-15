using Azure.Core;
using Ecommerce_project_API.Models;
using Ecommerce_project_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpGet("{ParentId}")]

        public async Task<ActionResult> GetProductByCategory(int ParentId)
        {

            var categoryExists = await _context.Category.AnyAsync(c => c.Id == ParentId);
            if (!categoryExists)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = "Invalid category ID.",
                    Status = 400,
                    IsSuccess = false,

                });
            }
            var data = await _context.Products
          .Where(c => c.CategoryId == ParentId)
          .ToListAsync();

            var response = new ApiResponse<List<Products>>
            {
                Message = "Products fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = data
            };
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> SearchProducts(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Message = "Search keyword is required.",
                    Status = 400,
                    IsSuccess = false
                });
            }

            var products = await _context.Products
                .Where(p => p.Name.Contains(keyword))
                .ToListAsync();

            foreach(var product in products)
            {
                product.SearchCount += 1;
            }
            await _context.SaveChangesAsync();
            var response = new ApiResponse<List<Products>>
            {
                Message = "Search results fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = products
            };

            return Ok(response);
        }



        [HttpGet]
        public async Task<ActionResult> GetMostSearchedProducts(int top = 10)
        {
            var products = await _context.Products
                .OrderByDescending(p => p.SearchCount)
                .Take(top)
                .ToListAsync();

            var response = new ApiResponse<List<Products>>
            {
                Message = "Most searched products fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = products
            };

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> BestSellers(int top = 50)
        {
            var products = await _context.Products
                .OrderByDescending(p => p.SoldQuantity) 
                .Take(top)
                .ToListAsync();

            var response = new ApiResponse<List<Products>>
            {
                Message = "Best seller products fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = products
            };

            return Ok(response);
        }


        [Authorize]
        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateProduct([FromForm] AddProductRequest request, int Id)
        {
            var product = await _context.Products.FindAsync(Id);
            if(product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Product Not Found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }
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


            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Quantity = request.Quantity;
            product.CategoryId = request.CategoryId;
            product.ImagePath = ImagePath;

          


            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Message = "Product updated successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });

        }

        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteProduct( int Id)
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
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object>
            {
                Message = "Product Deleted successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });
        }


        }
}
