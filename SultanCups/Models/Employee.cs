using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Employee
    {
        [Key]
        public int employee_id { get; set; }

        public string full_name { get; set; } = null!;

        public string? phone { get; set; }

        public string rank { get; set; } = null!;

        public decimal base_salary { get; set; }

        public string salary_mode { get; set; } = "راتب أساسي";
        public bool is_active { get; set; } = true;
        public List<Salary> Salaries { get; set; } = new();
    }
}
