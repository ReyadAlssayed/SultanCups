using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Purchase
    {

        [Key]
        public int purchase_id { get; set; }

        public int raw_material_id { get; set; }

        public int supplier_id { get; set; }

        public int quantity { get; set; }

        public decimal unit_price { get; set; }

        public DateTime purchase_date { get; set; }

        public DateTime? arrival_date { get; set; }

        public decimal customs_cost { get; set; }

        public decimal transport_cost { get; set; }

        public int? cash_box_id { get; set; }

        public string? notes { get; set; }
    }
}