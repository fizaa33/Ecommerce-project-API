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
        public async Task<ActionResult> GetAll()
        {
            var data = await _context.Users.ToListAsync();
            return Ok(data);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUserProfile()
        {
            int userId = _authService.GetUserIdFromToken(User);


            var user = await _context.Users.Where(u => u.Id == userId).Select(u => new { u.Id, u.Name, u.Email }).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound("You are not logged in");
            }
            return Ok(user);
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
            return Ok("Signup successful.");
        }


        [HttpPost]
        public async Task<ActionResult> Login(LoginUserRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                return Unauthorized("Invalid email or password");

            string token = _authService.GenerateJSONWebToken(user.Id);
            return Ok(new { Message = "Login Successful", Token = token });
        }




        [Authorize]
        [HttpPut("{Id}")]
        public async Task<ActionResult> Update(int Id, AddUserRequest request)
        {
            var user = await _context.Users.FindAsync(Id);
           
           
            if (user == null)
                return NotFound("User not found");
            user.Name = request.Name;
            user.Email = request.Email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);


            return Ok("User updated successfully.");
        }
        [Authorize]
        [HttpDelete("{Id}")]
        public async Task<ActionResult> Delete(int Id)
        {

            var user = await _context.Users.FindAsync(Id);

            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok("User deleted successfully.");
        }
    }
}
