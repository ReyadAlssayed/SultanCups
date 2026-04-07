using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Marketer
    {
        [Key]
        public int marketer_id { get; set; }

        public string name { get; set; } = string.Empty;

        public string? phone { get; set; }

        public string? address { get; set; }

        public decimal commission_per_box { get; set; }

        public string marketer_type { get; set; } = "خارجي"; // 👈 جديد

        public string? notes { get; set; } // 👈 جديد
        public bool is_active { get; set; } = true;
    }
}
