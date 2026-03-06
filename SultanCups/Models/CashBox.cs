using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class CashBox
    {
        [Key]
        public int cash_box_id { get; set; }

        public string name { get; set; } = null!;

        public bool is_active { get; set; }

        public List<Salary> Salaries { get; set; } = new();
    }
}