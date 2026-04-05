using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class RawMaterial
    {
        [Key]
        public int raw_material_id { get; set; }

        public string name { get; set; } = string.Empty;

        public decimal size { get; set; }

        public string unit_of_measure { get; set; } = string.Empty;

        public decimal unit_cost { get; set; }

        public bool is_active { get; set; } = true; 

        public string? notes { get; set; }
    }
}
