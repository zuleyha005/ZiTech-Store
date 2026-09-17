using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceMVC.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        [NotMapped]
        public string? Description { get; set; }
        public int CategoryId { get; set; }
    }
}