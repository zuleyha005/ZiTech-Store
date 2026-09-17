using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.Data;
using System.Net.Http.Json;
using ECommerceMVC.Models;
using ECommerceMVC.Services;

namespace ECommerceMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly JwtHttpClient _jwtHttpClient;

        public HomeController(
            ApplicationDbContext context,
            IHttpClientFactory httpClientFactory,
            JwtHttpClient jwtHttpClient)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _jwtHttpClient = jwtHttpClient;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .OrderBy(x => x.Id)
                .Take(50)
                .ToList();

            return View(products);
        }

        public IActionResult FeaturedProducts()
        {
            var products = _context.Products
                .OrderBy(x => x.Id)
                .Take(50)
                .ToList();

            return View(products);
        }
        public IActionResult Categories()
        {
            return View();
        }

        public IActionResult CategoryProducts(string name, int page = 1)
        {
            var category = _context.Categories
                .FirstOrDefault(x => x.Name == name);

            if (category == null)
            {
                return NotFound();
            }

            var products = _context.Products.AsQueryable();

            switch (category.Name)
            {
                case "Elektronik":
                    products = products.Where(x =>
                        x.Name.Contains("Laptop") ||
                        x.Name.Contains("Telefon") ||
                        x.Name.Contains("Tablet") ||
                        x.Name.Contains("Kulaklık") ||
                        x.Name.Contains("Kamera") ||
                        x.Name.Contains("Televizyon") ||
                        x.Name.Contains("Akıllı Saat") ||
                        x.Name.Contains("Konsol") ||
                        x.Name.Contains("Klavye") ||
                        x.Name.Contains("Mouse") ||
                        x.Name.Contains("SSD") ||
                        x.Name.Contains("Yazıcı") ||
                        x.Name.Contains("Powerbank") ||
                        x.Name.Contains("Şarj") ||
                        x.Name.Contains("Hoparlör") ||
                        x.Name.Contains("Monitör") ||
                        x.Name.Contains("USB"));
                    break;

                case "Bilgisayar":
                    products = products.Where(x =>
                        x.Name.Contains("Laptop") ||
                        x.Name.Contains("Bilgisayar") ||
                        x.Name.Contains("Mouse") ||
                        x.Name.Contains("Klavye") ||
                        x.Name.Contains("SSD"));
                    break;

                case "Telefon":
                    products = products.Where(x => x.Name.Contains("Telefon"));
                    break;

                case "Tablet":
                    products = products.Where(x => x.Name.Contains("Tablet"));
                    break;

                case "Kulaklık":
                    products = products.Where(x => x.Name.Contains("Kulaklık"));
                    break;

                case "Kamera":
                    products = products.Where(x =>
                        x.Name.Contains("Kamera") ||
                        x.Name.Contains("Fotoğraf"));
                    break;

                case "Televizyon":
                    products = products.Where(x => x.Name.Contains("Televizyon"));
                    break;

                case "Oyun":
                    products = products.Where(x =>
                        x.Name.Contains("Oyun") ||
                        x.Name.Contains("Oyuncu"));
                    break;

                case "Konsol":
                    products = products.Where(x =>
                        x.Name.Contains("Konsol"));
                    break;

                case "Bilgisayar Aksesuarları":
                    products = products.Where(x =>
                        x.Name.Contains("Mouse") ||
                        x.Name.Contains("Klavye") ||
                        x.Name.Contains("SSD") ||
                        x.Name.Contains("USB") ||
                        x.Name.Contains("Kulaklık"));
                    break;

                case "Ev ve Yaşam":
                    products = products.Where(x =>
                        x.Name.Contains("Masa") ||
                        x.Name.Contains("Sandalye") ||
                        x.Name.Contains("Lamba") ||
                        x.Name.Contains("Vazo") ||
                        x.Name.Contains("Dekorasyon") ||
                        x.Name.Contains("Mobilya") ||
                        x.Name.Contains("Ev") ||
                        x.Name.Contains("Bahçe"));
                    break;

                case "Mobilya":
                    products = products.Where(x =>
                        x.Name.Contains("Masa") ||
                        x.Name.Contains("Sandalye") ||
                        x.Name.Contains("Mobilya"));
                    break;

                case "Dekorasyon":
                    products = products.Where(x =>
                        x.Name.Contains("Dekorasyon") ||
                        x.Name.Contains("Vazo") ||
                        x.Name.Contains("Dekoratif") ||
                        x.Name.Contains("Duvar Saati") ||
                        x.Name.Contains("Masa Lambası") ||
                        x.Name.Contains("Aydınlatma"));
                    break;

                case "Aydınlatma":
                    products = products.Where(x =>
                        x.Name.Contains("Lamba") ||
                        x.Name.Contains("Aydınlatma"));
                    break;

                case "Bahçe":
                    products = products.Where(x =>
                        x.Name.Contains("Bahçe"));
                    break;

                case "Ofis":
                    products = products.Where(x =>
                        x.Name.Contains("Ofis") ||
                        x.Name.Contains("Çalışma Masası") ||
                        x.Name.Contains("Masa Lambası") ||
                        x.Name.Contains("Ofis Sandalyesi") ||
                        x.Name.Contains("Yazıcı") ||
                        x.Name.Contains("Kırtasiye") ||
                        x.Name.Contains("Klavye") ||
                        x.Name.Contains("Mouse"));
                    break;

                case "Kırtasiye":
                    products = products.Where(x =>
                        x.Name.Contains("Kırtasiye") ||
                        x.Name.Contains("Kalem") ||
                        x.Name.Contains("Defter"));
                    break;

                case "Kitap":
                    products = products.Where(x =>
                        x.Name.Contains("Kitap"));
                    break;

                case "Spor":
                    products = products.Where(x =>
                        x.Name.Contains("Spor") ||
                        x.Name.Contains("Fitness") ||
                        x.Name.Contains("Dambıl") ||
                        x.Name.Contains("Bisiklet"));
                    break;

                case "Fitness":
                    products = products.Where(x =>
                        x.Name.Contains("Fitness") ||
                        x.Name.Contains("Dambıl") ||
                        x.Name.Contains("Mat"));
                    break;

                case "Outdoor":
                    products = products.Where(x =>
                        x.Name.Contains("Outdoor"));
                    break;

                case "Giyim":
                    products = products.Where(x =>
                        x.Name.Contains("Giyim") ||
                        x.Name.Contains("Tişört") ||
                        x.Name.Contains("Pantolon"));
                    break;

                case "Kadın Giyim":
                    products = products.Where(x =>
                        x.Name.Contains("Kadın"));
                    break;

                case "Erkek Giyim":
                    products = products.Where(x =>
                        x.Name.Contains("Erkek"));
                    break;

                case "Çocuk Giyim":
                    products = products.Where(x =>
                        x.Name.Contains("Çocuk"));
                    break;

                case "Ayakkabı":
                    products = products.Where(x =>
                        x.Name.Contains("Ayakkabı"));
                    break;

                case "Çanta":
                    products = products.Where(x =>
                        x.Name.Contains("Çanta"));
                    break;

                case "Saat":
                    products = products.Where(x =>
                        x.Name.Contains("Saat"));
                    break;

                case "Takı":
                    products = products.Where(x =>
                        x.Name.Contains("Takı"));
                    break;

                case "Kozmetik":
                    products = products.Where(x =>
                        x.Name.Contains("Kozmetik") ||
                        x.Name.Contains("Makyaj"));
                    break;

                case "Kişisel Bakım":
                    products = products.Where(x =>
                        x.Name.Contains("Bakım") ||
                        x.Name.Contains("Şampuan"));
                    break;

                case "Sağlık":
                    products = products.Where(x =>
                        x.Name.Contains("Sağlık"));
                    break;

                case "Bebek":
                    products = products.Where(x =>
                        x.Name.Contains("Bebek"));
                    break;

                case "Oyuncak":
                    products = products.Where(x =>
                        x.Name.Contains("Oyuncak") ||
                        x.Name.Contains("Puzzle") ||
                        x.Name.Contains("Oyuncu"));
                    break;
                case "Evcil Hayvan":
                    products = products.Where(x =>
                        x.Name.Contains("Evcil Hayvan") ||
                        x.Name.Contains("Kedi") ||
                        x.Name.Contains("Köpek"));
                    break;

                case "Otomotiv":
                    products = products.Where(x =>
                        x.Name.Contains("Otomotiv") ||
                        x.Name.Contains("Araba"));
                    break;

                case "Yapı Market":
                    products = products.Where(x =>
                        x.Name.Contains("Yapı") ||
                        x.Name.Contains("Market"));
                    break;

                case "Müzik":
                    products = products.Where(x =>
                        x.Name.Contains("Müzik") ||
                        x.Name.Contains("Hoparlör"));
                    break;

                case "Film":
                    products = products.Where(x =>
                        x.Name.Contains("Film"));
                    break;

                case "Hobi":
                    products = products.Where(x =>
                        x.Name.Contains("Hobi"));
                    break;

                case "Sanat":
                    products = products.Where(x =>
                        x.Name.Contains("Sanat"));
                    break;

                case "Fotoğrafçılık":
                    products = products.Where(x =>
                        x.Name.Contains("Fotoğraf") ||
                        x.Name.Contains("Kamera"));
                    break;

                case "Seyahat":
                    products = products.Where(x =>
                        x.Name.Contains("Seyahat") ||
                        x.Name.Contains("Valiz"));
                    break;

                case "Valiz":
                    products = products.Where(x =>
                        x.Name.Contains("Valiz"));
                    break;

                case "Bisiklet":
                    products = products.Where(x =>
                        x.Name.Contains("Bisiklet"));
                    break;

                case "Motor Aksesuarları":
                    products = products.Where(x =>
                        x.Name.Contains("Motor"));
                    break;

                case "Akıllı Ev":
                    products = products.Where(x =>
                        x.Name.Contains("Akıllı Ev"));
                    break;

                case "Güvenlik":
                    products = products.Where(x =>
                        x.Name.Contains("Güvenlik"));
                    break;

                case "Elektrikli Ev Aletleri":
                    products = products.Where(x =>
                        x.Name.Contains("Elektrikli") ||
                        x.Name.Contains("Süpürge"));
                    break;

                case "Bahçe Mobilyaları":
                    products = products.Where(x =>
                        x.Name.Contains("Bahçe Mobilya"));
                    break;

                default:
                    products = products.Where(x =>
                        x.CategoryId == category.Id);
                    break;
            }

            const int pageSize = 25;

            if (page < 1)
            {
                page = 1;
            }

            var totalCount = products.Count();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize
            );

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var productList = products
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CategoryName = category.Name;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(productList);
        }

        public IActionResult OfferProducts(string type)
        {
            var products = _context.Products.AsQueryable();

            switch (type)
            {
                case "Laptop":
                    products = products.Where(x => x.Name.Contains("Laptop"));
                    break;

                case "Akıllı Saat":
                    products = products.Where(x => x.Name.Contains("Akıllı Saat"));
                    break;

                case "Kablosuz Kulaklık":
                    products = products.Where(x => x.Name.Contains("Kablosuz Kulaklık"));
                    break;

                case "Ofis Sandalyesi":
                    products = products.Where(x => x.Name.Contains("Ofis Sandalyesi"));
                    break;

                default:
                    return NotFound();
            }

            ViewBag.OfferType = type;

            return View(products.OrderBy(x => x.Id).ToList());
        }

        public async Task<IActionResult> AllProducts(
    int page = 1,
    string? search = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    string? sort = null,
    string? category = null)
        {
            const int pageSize = 25;

            if (page < 1)
            {
                page = 1;
            }

            var client = _jwtHttpClient.CreateClient();

            var url = $"Product/paged?page={page}&pageSize={pageSize}";

            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }

            if (minPrice.HasValue)
            {
                url += $"&minPrice={minPrice.Value}";
            }

            if (maxPrice.HasValue)
            {
                url += $"&maxPrice={maxPrice.Value}";
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                url += $"&sort={Uri.EscapeDataString(sort)}";
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                url += $"&category={Uri.EscapeDataString(category)}";
            }

            try
            {
                var result = await client.GetFromJsonAsync<PagedProductResponse>(url);

                if (result == null)
                {
                    ViewBag.ErrorMessage = "Ürün bilgileri alınamadı.";

                    ViewBag.CurrentPage = 1;
                    ViewBag.TotalPages = 0;
                    ViewBag.TotalCount = 0;

                    return View(new List<Product>());
                }
                ViewBag.CurrentPage = result.Page;
                ViewBag.TotalPages = result.TotalPages;
                ViewBag.TotalCount = result.TotalCount;

                ViewBag.Search = search;
                ViewBag.MinPrice = minPrice;
                ViewBag.MaxPrice = maxPrice;
                ViewBag.Sort = sort;
                ViewBag.Category = category;

                return View(result.Products);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Ürün sunucusuna şu anda ulaşılamıyor. Lütfen daha sonra tekrar deneyin.";

                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 0;
                ViewBag.TotalCount = 0;

                return View(new List<Product>());
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage =
                    "Ürünler yüklenirken beklenmeyen bir hata oluştu.";

                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 0;
                ViewBag.TotalCount = 0;

                return View(new List<Product>());
            }
        }
        public class PagedProductResponse
        {
            public List<Product> Products { get; set; } = new();

            public int TotalCount { get; set; }

            public int Page { get; set; }

            public int PageSize { get; set; }

            public int TotalPages { get; set; }
        }
    }
}