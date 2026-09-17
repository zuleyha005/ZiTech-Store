using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Net.Http.Json;
using ECommerceMVC.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerceMVC.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace ECommerceMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext context,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                // Admin bilgileri
                var adminLoginRequest = new
                {
                    Email = email,
                    Password = password
                };

                var adminResponse = await _httpClient.PostAsJsonAsync(
                    "http://localhost:5006/Admin/Login",
                    adminLoginRequest
                );

                if (adminResponse.IsSuccessStatusCode)
                {
                    var admin = await adminResponse.Content.ReadFromJsonAsync<Customer>();

                    if (admin != null)
                    {
                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, admin.Name ?? ""),
                    new Claim(ClaimTypes.Email, admin.Email ?? ""),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("AdminId", admin.Id.ToString())
                };

                        var identity = new ClaimsIdentity(
                            claims,
                            CookieAuthenticationDefaults.AuthenticationScheme
                        );

                        var principal = new ClaimsPrincipal(identity);

                        // JWT oluştur
                        var secretKey = _configuration["JwtSettings:SecretKey"];

                        if (string.IsNullOrEmpty(secretKey))
                        {
                            throw new InvalidOperationException(
                                "JWT SecretKey bulunamadı."
                            );
                        }

                        var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(secretKey)
                        );

                        var credentials = new SigningCredentials(
                            key,
                            SecurityAlgorithms.HmacSha256
                        );

                        var token = new JwtSecurityToken(
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: credentials
                        );

                        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                        Response.Cookies.Append("JwtToken", jwt, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Lax,
                            Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                        });

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            principal
                        );

                        return RedirectToAction("Index", "Admin");
                    }
                }

                // Normal kullanıcı girişi
                var loginRequest = new
                {
                    Email = email,
                    Password = password
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "http://localhost:5006/Customer/Login",
                    loginRequest
                );

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "E-posta veya şifre hatalı.";
                    return View();
                }

                var loginResponse =
                    await response.Content.ReadFromJsonAsync<CustomerLoginResponse>();

                if (loginResponse == null)
                {
                    ViewBag.Error = "Kullanıcı bilgileri alınamadı.";
                    return View();
                }

                var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, loginResponse.Name ?? ""),
            new Claim(ClaimTypes.Email, loginResponse.Email ?? ""),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("CustomerId", loginResponse.Id.ToString())
        };

                var userIdentity = new ClaimsIdentity(
                    userClaims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                var userPrincipal = new ClaimsPrincipal(userIdentity);

                if (!string.IsNullOrEmpty(loginResponse.Token))
                {
                    Response.Cookies.Append("JwtToken", loginResponse.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                    });
                }

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    userPrincipal
                );

                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                ViewBag.Error =
                    "Sunucuya şu anda ulaşılamıyor. Lütfen daha sonra tekrar deneyin.";

                return View();
            }
            catch (Exception)
            {
                ViewBag.Error =
                    "Giriş yapılırken beklenmeyen bir hata oluştu.";

                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            var customer = new Customer
            {
                Name = name,
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(
                "http://localhost:5006/Customer",
                customer
            );
            Console.WriteLine(response);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ViewBag.Error = "Bu e-posta adresi zaten kayıtlı.";
                return View();
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                ViewBag.Error = "Kayıt sırasında bir hata oluştu: " + errorMessage;

                return View();
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult MyAccount()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var customerId = User.FindFirst("CustomerId")?.Value;
            var name = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            ViewBag.CustomerId = customerId;
            ViewBag.Name = name;
            ViewBag.Email = email;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var customerIdClaim = User.FindFirst("CustomerId")?.Value;

            if (!int.TryParse(customerIdClaim, out int customerId))
            {
                return RedirectToAction("Login");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Index", "Home");
        }
        public class CustomerLoginResponse
        {
            public string? Token { get; set; }
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Email { get; set; }
        }
    }
}