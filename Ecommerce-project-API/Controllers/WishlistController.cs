using Azure.Core;
using Ecommerce_project_API.Models;
using Ecommerce_project_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_project_API.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly AuthService _authService;
        private myDbContext _context;

        public WishlistController(myDbContext context, AuthService authService)
        {

            _context = context;
            _authService = authService;

        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUserWishlist()
        {
            int userId = _authService.GetUserIdFromToken(User);

            var data = await _context.Wishlist
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .ToListAsync();
 
            var response = new ApiResponse<List<Wishlist>>
            {
                Message = "Wishlist fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Data = data
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]

        public async Task<ActionResult> AddToWishlist(AddWishlistRequest request)
        {
            int userId = _authService.GetUserIdFromToken(User);

            var exists = await _context.Wishlist.AnyAsync(w =>
                w.ProductId == request.ProductId &&
                w.UserId == userId);
            if (exists)
                return BadRequest(new ApiResponse<object>
                {
                    Message = "item is already in wishlist",
                    Status = 400,
                    IsSuccess = false,
                    Data = null
                });

            var wishlistItem = new Wishlist
            {
                ProductId = request.ProductId,
                UserId = userId,
                Quantity = request.Quantity,
                AddedAt = DateTime.UtcNow
            };

            _context.Wishlist.Add(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Wishlist>
            {
                Message = "Item added to wishlist.",
                Status = 200,
                IsSuccess = true,
                Data = null
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateWishlistItem(AddWishlistRequest request, int id)
        {
            int userId = _authService.GetUserIdFromToken(User);

            var wishlistItem = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (wishlistItem == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "Item not found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }

            // Update quantity only
            wishlistItem.Quantity = request.Quantity;

            _context.Wishlist.Update(wishlistItem);
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<Wishlist>
            {
                Message = "Wishlist item updated successfully.",
                Status = 200,
                IsSuccess = true,
                Data = wishlistItem
            });
        }

     

        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteWishlist(int Id)
        {

            var wishlist = await _context.Wishlist.FindAsync(Id);

            if (wishlist == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "wishlist item Not Found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }

            _context.Wishlist.Remove(wishlist);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object>
            {
                Message = "wishlist item Deleted successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });
        }


    }
}
