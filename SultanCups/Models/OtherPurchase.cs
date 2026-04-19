using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class OtherPurchase
    {
        [Key]
        public int other_purchase_id { get; set; }

        public string name { get; set; } = string.Empty;

        public int quantity { get; set; } = 1;

        public decimal cost { get; set; }

        public DateTime purchase_date { get; set; }

        public string? notes { get; set; }

        public int? cash_box_id { get; set; }
    }
}