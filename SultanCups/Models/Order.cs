using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Order
    {
        [Key]
        public int order_id { get; set; }

        public string? order_number { get; set; }

        public int marketer_id { get; set; }

        public int product_id { get; set; }

        public int box_count { get; set; }

        public decimal unit_price { get; set; }

        public decimal commission_per_box { get; set; }

        public DateTime order_date { get; set; }

        public string? notes { get; set; }
    }
}