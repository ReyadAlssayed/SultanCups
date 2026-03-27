using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Supplier
    {
        [Key]
        public int supplier_id { get; set; }

        public string name { get; set; } = string.Empty;

        public string? phone { get; set; }

        public string? email { get; set; }

        public string? location { get; set; }
        public bool is_active { get; set; } = true;
        public string? notes { get; set; }
    }
}