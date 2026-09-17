using Microsoft.AspNetCore.Mvc;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace CustomerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController : ControllerBase
{
    private readonly CustomerDbContext _context;
    private readonly PasswordHasher<Customer> _passwordHasher = new PasswordHasher<Customer>();

    public CustomerController(CustomerDbContext context)
    {
        _context = context;
    }
    
    [Authorize]
    [HttpGet]
    public IActionResult GetCustomers()
    {
        var customers = _context.Customers.ToList();

        return Ok(customers);
    }
    
    [AllowAnonymous]
    [HttpPost]
    public IActionResult AddCustomer(Customer customer)
    {
        var existingCustomer = _context.Customers
            .FirstOrDefault(c =>
                c.Email != null &&
                customer.Email != null &&
                c.Email.ToLower() == customer.Email.Trim().ToLower());

        if (existingCustomer != null)
        {
            return Conflict("Bu e-posta adresi zaten kayıtlı.");
        }

        customer.Email = customer.Email?.Trim();

        customer.Password = _passwordHasher.HashPassword(
            customer,
            customer.Password ?? ""
        );

        _context.Customers.Add(customer);
        _context.SaveChanges();

        return Ok(customer);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCustomer(int id, Customer updatedCustomer)
    {
        var customer = _context.Customers.Find(id);

        if (customer == null)
        {
            return NotFound();
        }

        customer.Name = updatedCustomer.Name;
        customer.Email = updatedCustomer.Email;
        if (!string.IsNullOrWhiteSpace(updatedCustomer.Password))
        {
            customer.Password = _passwordHasher.HashPassword(
                customer,
                updatedCustomer.Password
            );
        }
        _context.SaveChanges();

        return Ok(customer);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(int id)
    {
        var customer = _context.Customers.Find(id);

        if (customer == null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        _context.SaveChanges();

        return Ok(customer);
    }

    [HttpPost("Login")]
    public IActionResult Login(CustomerLoginRequest request)
    {
        var customer = _context.Customers
            .FirstOrDefault(c =>
                c.Email != null &&
                request.Email != null &&
                c.Email.ToLower() == request.Email.Trim().ToLower());

        if (customer == null)
        {
            return Unauthorized("E-posta veya şifre hatalı.");
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            customer,
            customer.Password ?? "",
            request.Password ?? ""
        );

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized("E-posta veya şifre hatalı.");
        }

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
        new Claim(ClaimTypes.Name, customer.Name ?? ""),
        new Claim(ClaimTypes.Email, customer.Email ?? ""),
        new Claim(ClaimTypes.Role, "User")
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("ECommerceMVC-JWT-Secret-Key-2026")
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = jwt,
            customer.Id,
            customer.Name,
            customer.Email
        });
    }
}