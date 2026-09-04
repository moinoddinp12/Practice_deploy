namespace Online_Car_Marketplace.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int CarId { get; set; }

        public Car Car { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
