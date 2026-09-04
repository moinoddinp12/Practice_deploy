using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Car_Marketplace.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public String Make { get; set; }

        [Required, StringLength(60)]
        public String Model { get; set; }

        [Range(1990, 2100)]
        public int year { get; set; }

        [Range(0, 2_000_000)]
        public int? Mileage { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(1, 100000000)]
        public decimal price { get; set; }

        [StringLength(2000)]
        public string? Description { get; set; }

        [StringLength(60)]
        public string? City { get; set; }

        [StringLength(260)]
        public string? ImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public void Touch() => UpdatedAt = DateTime.UtcNow;

        public string? CarImage { set; get; }

    }
}
