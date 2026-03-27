using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Product
    {
        [Key]
        public int product_id { get; set; }

        public string name { get; set; } = string.Empty;
        public bool is_active { get; set; } = true;
    }
}
