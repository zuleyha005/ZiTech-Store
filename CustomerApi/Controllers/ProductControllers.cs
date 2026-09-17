using Microsoft.AspNetCore.Mvc;
using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CustomerApi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly CustomerDbContext _context;
    private readonly IMemoryCache _cache;
    private long GetProductCacheVersion()
    {
        const string versionKey = "products_cache_version";

        if (_cache.TryGetValue(versionKey, out long version))
        {
            return version;
        }

        version = 1;

        _cache.Set(versionKey, version);

        return version;
    }
    private void InvalidateProductCache()
    {
        const string versionKey = "products_cache_version";

        var currentVersion = GetProductCacheVersion();

        _cache.Set(versionKey, currentVersion + 1);
    }

    public ProductController(
        CustomerDbContext context,
        IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    // Ürünleri getir
    [HttpGet]
    public IActionResult GetProducts()
    {
        var version = GetProductCacheVersion();
        var cacheKey = $"products_all_v{version}";

        if (_cache.TryGetValue(cacheKey, out List<Product>? cachedProducts))
        {
            return Ok(cachedProducts);
        }

        var products = _context.Products
            .OrderBy(x => x.Id)
            .ToList();

        _cache.Set(
            cacheKey,
            products,
            TimeSpan.FromMinutes(5)
        );

        return Ok(products);
    }

    // ID'ye göre ürün getir
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        var version = GetProductCacheVersion();
        var cacheKey = $"product_{id}_v{version}";

        if (_cache.TryGetValue(cacheKey, out Product? cachedProduct))
        {
            return Ok(cachedProduct);
        }

        var product = _context.Products
            .FirstOrDefault(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        _cache.Set(
            cacheKey,
            product,
            TimeSpan.FromMinutes(5)
        );

        return Ok(product);
    }

    // Ürünleri sayfalı şekilde getir
    [HttpGet("paged")]
    public IActionResult GetProductsPaged(
    int page = 1,
    int pageSize = 1000,
    string? search = null,
    decimal? minPrice = null,
    decimal? maxPrice = null,
    string? sort = null,
    string? category = null)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 1000;
        }

        var version = GetProductCacheVersion();

        var cacheKey =
            $"products_paged_v{version}_" +
            $"page:{page}_" +
            $"pageSize:{pageSize}_" +
            $"search:{search?.Trim().ToLowerInvariant()}_" +
            $"minPrice:{minPrice}_" +
            $"maxPrice:{maxPrice}_" +
            $"sort:{sort}_" +
            $"category:{category?.Trim().ToLowerInvariant()}";
        if (_cache.TryGetValue(cacheKey, out object? cachedResult))
        {
            return Ok(cachedResult);
        }

        var products = _context.Products.AsQueryable();

        // Ürün adına göre arama
        if (!string.IsNullOrWhiteSpace(search))
        {
            products = products.Where(x =>
                x.Name != null &&
                x.Name.ToLower().Contains(search.ToLower()));
        }

        // Minimum fiyat
        if (minPrice.HasValue)
        {
            products = products.Where(x =>
                x.Price >= minPrice.Value);
        }

        // Maksimum fiyat
        if (maxPrice.HasValue)
        {
            products = products.Where(x =>
                x.Price <= maxPrice.Value);
        }

        // Kategori
        if (!string.IsNullOrWhiteSpace(category))
        {
            switch (category)
            {
                case "Elektronik":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
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
                            x.Name.Contains("USB")
                        ));
                    break;

                case "Bilgisayar":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Laptop") ||
                            x.Name.Contains("Bilgisayar") ||
                            x.Name.Contains("Mouse") ||
                            x.Name.Contains("Klavye") ||
                            x.Name.Contains("SSD")
                        ));
                    break;

                case "Telefon":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Telefon"));
                    break;

                case "Tablet":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Tablet"));
                    break;

                case "Kulaklık":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Kulaklık"));
                    break;

                case "Kamera":
                    products = products.Where(x =>
                        x.Name != null &&
                        (x.Name.Contains("Kamera") ||
                         x.Name.Contains("Fotoğraf")));
                    break;

                case "Televizyon":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Televizyon"));
                    break;

                case "Konsol":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Konsol"));
                    break;

                case "Ev ve Yaşam":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Masa") ||
                            x.Name.Contains("Sandalye") ||
                            x.Name.Contains("Lamba") ||
                            x.Name.Contains("Vazo") ||
                            x.Name.Contains("Dekorasyon") ||
                            x.Name.Contains("Mobilya") ||
                            x.Name.Contains("Ev") ||
                            x.Name.Contains("Bahçe")
                        ));
                    break;

                case "Mobilya":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Masa") ||
                            x.Name.Contains("Sandalye") ||
                            x.Name.Contains("Mobilya")
                        ));
                    break;

                case "Dekorasyon":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Dekorasyon") ||
                            x.Name.Contains("Vazo") ||
                            x.Name.Contains("Dekoratif") ||
                            x.Name.Contains("Duvar Saati") ||
                            x.Name.Contains("Masa Lambası") ||
                            x.Name.Contains("Aydınlatma")
                        ));
                    break;

                case "Aydınlatma":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Lamba") ||
                            x.Name.Contains("Aydınlatma")
                        ));
                    break;

                case "Bahçe":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Bahçe"));
                    break;

                case "Ofis":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Ofis") ||
                            x.Name.Contains("Çalışma Masası") ||
                            x.Name.Contains("Masa Lambası") ||
                            x.Name.Contains("Ofis Sandalyesi") ||
                            x.Name.Contains("Yazıcı") ||
                            x.Name.Contains("Kırtasiye") ||
                            x.Name.Contains("Klavye") ||
                            x.Name.Contains("Mouse")
                        ));
                    break;

                case "Kırtasiye":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Kırtasiye") ||
                            x.Name.Contains("Kalem") ||
                            x.Name.Contains("Defter")
                        ));
                    break;

                case "Kitap":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Kitap"));
                    break;

                case "Spor":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Spor") ||
                            x.Name.Contains("Fitness") ||
                            x.Name.Contains("Dambıl") ||
                            x.Name.Contains("Bisiklet")
                        ));
                    break;

                case "Fitness":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Fitness") ||
                            x.Name.Contains("Dambıl") ||
                            x.Name.Contains("Mat")
                        ));
                    break;

                case "Giyim":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Giyim") ||
                            x.Name.Contains("Tişört") ||
                            x.Name.Contains("Pantolon")
                        ));
                    break;

                case "Kadın Giyim":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Kadın"));
                    break;

                case "Erkek Giyim":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Erkek"));
                    break;

                case "Çocuk Giyim":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Çocuk"));
                    break;

                case "Ayakkabı":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Ayakkabı"));
                    break;

                case "Çanta":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Çanta"));
                    break;

                case "Saat":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Saat"));
                    break;

                case "Takı":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Takı"));
                    break;

                case "Kozmetik":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Kozmetik") ||
                            x.Name.Contains("Makyaj")
                        ));
                    break;

                case "Kişisel Bakım":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Bakım") ||
                            x.Name.Contains("Şampuan")
                        ));
                    break;

                case "Oyuncak":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Oyuncak") ||
                            x.Name.Contains("Puzzle") ||
                            x.Name.Contains("Oyuncu")
                        ));
                    break;

                case "Evcil Hayvan":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Evcil Hayvan") ||
                            x.Name.Contains("Kedi") ||
                            x.Name.Contains("Köpek")
                        ));
                    break;

                case "Otomotiv":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Otomotiv") ||
                            x.Name.Contains("Araba")
                        ));
                    break;

                case "Müzik":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Müzik") ||
                            x.Name.Contains("Hoparlör")
                        ));
                    break;

                case "Film":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Film"));
                    break;

                case "Hobi":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Hobi"));
                    break;

                case "Sanat":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Sanat"));
                    break;

                case "Fotoğrafçılık":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Fotoğraf") ||
                            x.Name.Contains("Kamera")
                        ));
                    break;

                case "Seyahat":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Seyahat") ||
                            x.Name.Contains("Valiz")
                        ));
                    break;

                case "Valiz":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Valiz"));
                    break;

                case "Bisiklet":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Bisiklet"));
                    break;

                case "Motor Aksesuarları":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Motor"));
                    break;

                case "Akıllı Ev":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Akıllı Ev"));
                    break;

                case "Güvenlik":
                    products = products.Where(x =>
                        x.Name != null && x.Name.Contains("Güvenlik"));
                    break;

                case "Elektrikli Ev Aletleri":
                    products = products.Where(x =>
                        x.Name != null &&
                        (
                            x.Name.Contains("Elektrikli") ||
                            x.Name.Contains("Süpürge")
                        ));
                    break;
            }
        }

        // Sıralama
        switch (sort)
        {
            case "priceAsc":
                products = products.OrderBy(x => x.Price);
                break;

            case "priceDesc":
                products = products.OrderByDescending(x => x.Price);
                break;

            case "nameAsc":
                products = products.OrderBy(x => x.Name);
                break;

            case "nameDesc":
                products = products.OrderByDescending(x => x.Name);
                break;

            default:
                products = products.OrderBy(x => x.Id);
                break;
        }

        // Filtrelerden sonra toplam ürün sayısı
        var totalCount = products.Count();

        // Pagination
        var productList = products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var totalPages = (int)Math.Ceiling(
            (double)totalCount / pageSize
        );

        var result = new
        {
            products = productList,
            totalCount,
            page,
            pageSize,
            totalPages
        };

        _cache.Set(
            cacheKey,
            result,
            TimeSpan.FromMinutes(5)
        );

        return Ok(result);
    }

    // Ürün ekle
    [HttpPost]
    public IActionResult AddProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();

        InvalidateProductCache();

        return Ok(product);
    }

    // Ürün güncelle
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, Product updatedProduct)
    {
        var product = _context.Products.FirstOrDefault(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;
        product.CategoryId = updatedProduct.CategoryId;

        _context.SaveChanges();

        InvalidateProductCache();

        return Ok(product);
    }

    // Ürün sil
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        _context.SaveChanges();

        InvalidateProductCache();

        return Ok(product);
    }

}