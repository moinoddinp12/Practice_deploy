using System.ComponentModel.DataAnnotations;

namespace Online_Car_Marketplace.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required,StringLength(50)]
        public string Username { get; set; }

        [Required,StringLength(50)]
        public string Password { get; set; }

        [Required,StringLength (50)]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } = "User";
    }
}
