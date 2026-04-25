using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SultanCups.Models
{
    public class FinancialEvent
    {
        [Key]
        public int event_id { get; set; }

        public string event_type { get; set; } = null!;

        public string direction { get; set; } = null!;

        public decimal amount { get; set; }

        public int cash_box_id { get; set; }

        // المسؤول المنفذ
        public int performed_by { get; set; }
        public string? admin_name_snapshot { get; set; }

        // مرجع الحركة
        public string? ref_table { get; set; }
        public int? ref_id { get; set; }
        public string? ref_code { get; set; }

        // الشخص المرتبط
        public int? person_id { get; set; }
        public string? person_name_snapshot { get; set; }

        // العنصر المرتبط
        public int? item_id { get; set; }
        public string? item_name_snapshot { get; set; }

        public DateTime event_date { get; set; }

        public string? notes { get; set; }

        [ForeignKey("cash_box_id")]
        public CashBox CashBox { get; set; } = null!;

        [ForeignKey("performed_by")]
        public Admain Admin { get; set; } = null!;
    }
}