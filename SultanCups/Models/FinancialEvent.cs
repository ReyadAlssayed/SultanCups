using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SultanCups.Models
{
    public class FinancialEvent
    {
        [Key]
        public int event_id { get; set; }

        // نوع العملية (دفع راتب، تحصيل، ...)
        public string event_type { get; set; } = null!;

        // IN / OUT
        public string direction { get; set; } = null!;

        // القيمة
        public decimal amount { get; set; }

        // الخزنة
        public int cash_box_id { get; set; }

        // من قام بالعملية
        public int performed_by { get; set; }

        // ربط اختياري
        public string? ref_table { get; set; }
        public int? ref_id { get; set; }
        public string? ref_code { get; set; }

        public DateTime event_date { get; set; }

        public string? notes { get; set; }

        // Navigation
        [ForeignKey("cash_box_id")]
        public CashBox CashBox { get; set; } = null!;

        [ForeignKey("performed_by")]
        public Admain Admin { get; set; } = null!;
        public int? employee_id { get; set; }
        public string? person_name_snapshot { get; set; }
    }
}