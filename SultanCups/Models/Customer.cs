using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Customer
    {
        [Key]
        public int customer_id { get; set; }

        public string name { get; set; } = string.Empty;

        public string? phone { get; set; }

        public string? address { get; set; }

        public string? notes { get; set; }

        public bool is_active { get; set; } = true;
    }
}