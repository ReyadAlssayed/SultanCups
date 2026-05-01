namespace SultanCups.Models
{
    public class OrderView
    {
        public int order_id { get; set; }
        public string person_type { get; set; } = "";
        public string person_name { get; set; } = "";

        public int items_count { get; set; }

        public decimal total { get; set; }
        public decimal discount_total { get; set; }
        public decimal net_total { get; set; }

        public decimal commission_total { get; set; }

        public decimal paid_amount { get; set; }   // 🔥 أضف هذا

        public DateTime order_date { get; set; }
    }
}