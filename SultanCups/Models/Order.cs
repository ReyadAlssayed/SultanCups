using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Order
    {
        [Key]
        public int order_id { get; set; }

        public int person_id { get; set; } // زبون أو مسوق
        public string person_type { get; set; } = ""; // customer / marketer

        public string sale_type { get; set; } = ""; // direct / marketer

        public decimal discount_total { get; set; }

        public decimal commission_per_box { get; set; }
        public bool commission_paid { get; set; }

        public DateTime order_date { get; set; }

        public string? notes { get; set; }

        // 🔗 تفاصيل الفاتورة
        public List<OrderItem> Items { get; set; } = new();
    }
}