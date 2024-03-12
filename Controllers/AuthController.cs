using apinet.Data;
using apinet.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace apinet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;


        public AuthController(IConfiguration config, DataContext context)
        {
            _config = config;
            _context = context;
        }
        [HttpPost("register")]
        public ActionResult<User> Register(User request)
        {
            if (!IsValidEmail(request.Email))
            {
                return BadRequest("Invalid email");
            }
            if (_context.Users.Any(x => x.Email == request.Email))
            {
                return BadRequest("Email already exists");
            }
            if (!IsValidPassword(request.Password))
            {
                return BadRequest("Need at least 6ch & 1 number");
            }

            // Create new user
            User newUser = new User()
            {
                Name = request.Name,
                Email = request.Email,
                City = request.City,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Role = request.Role,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new { message = "Registration successful" });
        }
        [HttpPost("login")]
        public ActionResult<User> Login(User request)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == request.Email);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return BadRequest("Wrong password");
            }

            string token = CreateToken(user);
            return Ok(token);
        }
        private static bool IsValidEmail(string email)
        {
            string emailPattern = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";
            Regex regex = new(emailPattern);
            return regex.IsMatch(email);
        }
        private static bool IsValidPassword(string password)
        {
            return password.Length >= 6 && password.Any(char.IsDigit);
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.StreetAddress, user.Address),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim(ClaimTypes.Locality, user.City),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Key").Value!));

            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: signingCredentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}