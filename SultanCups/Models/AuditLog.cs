using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SultanCups.Models
{
    public class AuditLog
    {
        [Key]
        public int audit_id { get; set; }

        public string table_name { get; set; } = null!;

        public string operation { get; set; } = null!;

        public string? record_id { get; set; }

        [Column(TypeName = "jsonb")]
        public string? old_data { get; set; }

        [Column(TypeName = "jsonb")]
        public string? new_data { get; set; }

        public int performed_by { get; set; }

        public DateTime performed_at { get; set; }

        [ForeignKey("performed_by")]
        public Admain Admin { get; set; } = null!;
    }
}