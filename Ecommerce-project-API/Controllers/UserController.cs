using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using Ecommerce_project_API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce_project_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure;

namespace Ecommerce_project_API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly AuthService _authService;
        private myDbContext _context;

        public UserController(myDbContext context, AuthService authService)
        {

            _context = context;
            _authService = authService;

        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            var data = await _context.Users.ToListAsync();

            var response = new ApiResponse<List<UserModel>>
            {
                Message = "Users get successfully.",
                Status = 200,
                IsSuccess = true,
                Data = data
            };
            return Ok(response);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUserProfile()
        {
            int userId = _authService.GetUserIdFromToken(User);


            var user = await _context.Users.Where(u => u.Id == userId).Select(u => new { u.Id, u.Name, u.Email }).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "You are not logged in",
                    Status = 404,
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid ID", "Token is misssing" },

                    Data = null
                });
            }

            var response = new ApiResponse<object>
            {
                Message = "User fetched successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,

                Data = user
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Signup(AddUserRequest request)
        {

            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return BadRequest("User already exists.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

           var user = new UserModel
            {
              Name =  request.Name,
              Email =  request.Email,
                Password = hashedPassword
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok( new ApiResponse<object>
            {
                Message = "User registered successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,

                Data = null
            });
        }


        [HttpPost]
        public async Task<ActionResult> Login(LoginUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid email or password");

            string token = _authService.GenerateJSONWebToken(user.Id);
            return Ok( new ApiResponse<object>
            {
                Message = "User Login successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,

                Data = token
            });
        }




        [Authorize]
        [HttpPut("{Id}")]
        public async Task<ActionResult> UpdateUser(int Id, AddUserRequest request)
        {
            var user = await _context.Users.FindAsync(Id);
           
           
           if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "User Not Found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }

            user.Name = request.Name;
            user.Email = request.Email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return Ok( new ApiResponse<object>
            {
                Message = "User Updated successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });
        }
        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> DeleteUser(int Id)
        {

            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Message = "User Not Found",
                    Status = 404,
                    IsSuccess = false,
                    Data = null
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new ApiResponse<object>
            {
                Message = "User Deleted successfully.",
                Status = 200,
                IsSuccess = true,
                Errors = null,
                Data = null
            });
        }
    }
}
