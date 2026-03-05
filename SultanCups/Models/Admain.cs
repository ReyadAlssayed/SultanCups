using System.ComponentModel.DataAnnotations;

namespace SultanCups.Models
{
    public class Admain
    {
        [Key]
        public int admin_id { get; set; }

        public string full_name { get; set; }

        public string username { get; set; }

        public string password_hash { get; set; }

        public string role { get; set; }

        public string phone { get; set; }

        public bool is_active { get; set; }

        public DateTime created_at { get; set; }
    }
}