using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SultanCups.Models
{
    public class ProductStock
    {
        [Key]
        [ForeignKey("Product")]
        public int product_id { get; set; }

        public int quantity { get; set; }

        public Product Product { get; set; } = null!;
    }
}