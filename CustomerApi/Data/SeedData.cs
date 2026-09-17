using Bogus;
using System.Linq;
using CustomerApi.Controllers;
using CustomerApi.Models;

namespace CustomerApi.Data
{
    public static class SeedData
    {
        public static void Initialize(CustomerDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categoryNames = new List<string>
{
    "Elektronik",
    "Bilgisayar",
    "Telefon",
    "Tablet",
    "Kulaklık",
    "Kamera",
    "Televizyon",
    "Oyun",
    "Konsol",
    "Bilgisayar Aksesuarları",
    "Ev ve Yaşam",
    "Mobilya",
    "Dekorasyon",
    "Aydınlatma",
    "Bahçe",
    "Ofis",
    "Kırtasiye",
    "Kitap",
    "Spor",
    "Fitness",
    "Outdoor",
    "Giyim",
    "Kadın Giyim",
    "Erkek Giyim",
    "Çocuk Giyim",
    "Ayakkabı",
    "Çanta",
    "Saat",
    "Takı",
    "Kozmetik",
    "Kişisel Bakım",
    "Sağlık",
    "Bebek",
    "Oyuncak",
    "Evcil Hayvan",
    "Otomotiv",
    "Yapı Market",
    "Müzik",
    "Film",
    "Hobi",
    "Sanat",
    "Fotoğrafçılık",
    "Seyahat",
    "Valiz",
    "Bisiklet",
    "Motor Aksesuarları",
    "Akıllı Ev",
    "Güvenlik",
    "Elektrikli Ev Aletleri",
    "Bahçe Mobilyaları"
};

                var categories = categoryNames
                    .Select(name => new Category { Name = name })
                    .ToList();

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
            if (!context.Products.Any())
            {
                var productNames = new List<string>
{
    "Laptop",
    "Akıllı Telefon",
    "Tablet",
    "Kablosuz Kulaklık",
    "Bluetooth Hoparlör",
    "Oyuncu Klavyesi",
    "Oyuncu Mouse",
    "Monitör",
    "Web Kamera",
    "Akıllı Saat",
    "Televizyon",
    "Fotoğraf Makinesi",
    "Yazıcı",
    "SSD Disk",
    "USB Bellek",
    "Laptop Çantası",
    "Telefon Kılıfı",
    "Şarj Cihazı",
    "Powerbank",
    "Masa Lambası",
    "Kitaplık",
    "Çalışma Masası",
    "Ofis Sandalyesi",
    "Duvar Saati",
    "Dekoratif Vazo",
    "Spor Ayakkabı",
    "Sırt Çantası",
    "Kol Saati",
    "Güneş Gözlüğü",
    "Parfüm",
    "Şampuan",
    "Fitness Matı",
    "Dambıl",
    "Bisiklet",
    "Kamera Tripodu",
    "Oyuncak Araba",
    "Puzzle",
    "Kitap",
    "Valiz",
    "Elektrikli Süpürge"
};

                var productFaker = new Faker<Product>()
                .RuleFor(p => p.Name, f => $"{f.PickRandom(productNames)} {f.Random.Number(1000, 999999)}")
                .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(10, 5000), 2))
                .RuleFor(p => p.CategoryId, f => f.Random.Int(1, 50));

                var products = productFaker.Generate(10000);

                context.Products.AddRange(products);
                context.SaveChanges();
            }
            if (!context.Customers.Any())
            {
                var customerFaker = new Faker<Customer>()
                    .RuleFor(c => c.Name, f => f.Name.FullName())
                    .RuleFor(c => c.Email, f => f.Internet.Email())
                    .RuleFor(c => c.Password, f => f.Internet.Password());

                var customers = customerFaker.Generate(5000);

                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
            var customersWithoutPassword = context.Customers
    .Where(c => c.Password == null)
    .ToList();

            foreach (var customer in customersWithoutPassword)
            {
                customer.Password = new Faker().Internet.Password();
            }

            context.SaveChanges();

            if (!context.Carts.Any())
            {
                var customerIds = context.Customers.Select(x => x.Id).ToList();
                var productIds = context.Products.Select(x => x.Id).ToList();

                var cartFaker = new Faker<Cart>()
                    .RuleFor(c => c.CustomerId, (f, c) => f.PickRandom(customerIds))
                    .RuleFor(c => c.ProductId, (f, c) => f.PickRandom(productIds))
                    .RuleFor(c => c.Quantity, (f, c) => f.Random.Int(1, 10));

                var carts = cartFaker.Generate(1000);

                context.Carts.AddRange(carts);
                context.SaveChanges();
            }
        }
    }
}