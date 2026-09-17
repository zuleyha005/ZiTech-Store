using Microsoft.AspNetCore.Mvc;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Identity;

namespace CustomerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly CustomerDbContext _context;
        private readonly PasswordHasher<Admin> _passwordHasher;

        public AdminController(CustomerDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Admin>();
        }

        [HttpPost]
        public IActionResult AddAdmin(Admin admin)
        {
            admin.Password = _passwordHasher.HashPassword(
                admin,
                admin.Password ?? ""
            );

            _context.Admins.Add(admin);
            _context.SaveChanges();

            return Ok(admin);
        }
        [HttpPost("Login")]
        public IActionResult Login(AdminLoginRequest request)
        {
            var admin = _context.Admins.FirstOrDefault(x =>
                x.Email == request.Email);

            if (admin == null)
            {
                return Unauthorized();
            }

            var result = _passwordHasher.VerifyHashedPassword(
                admin,
                admin.Password ?? "",
                request.Password ?? ""
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                admin.Id,
                admin.Name,
                admin.Email
            });
        }
    }
}