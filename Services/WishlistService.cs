using Online_Car_Marketplace.Data;
using Online_Car_Marketplace.Models;
using Online_Car_Marketplace.Services.Interfaces;


namespace Online_Car_Marketplace.Services
{
  
        public class WishlistService : IWishlistService
        {
            private readonly ApplicationDbContext _context;

            public WishlistService(ApplicationDbContext context)
            {
                _context = context;
            }

            public string AddToWishlist(int userId, int CarId)
            {
                var exists = _context.Wishlist
                    .FirstOrDefault(w => w.UserId == userId && w.CarId == CarId);

                if (exists != null)
                    return "Car already in wishlist";

                var wishlist = new Wishlist
                {
                    UserId = userId,
                    CarId = CarId,
                    CreatedAt = DateTime.Now
                };

                _context.Wishlist.Add(wishlist);
                _context.SaveChanges();

                return "Added to wishlist";
            }

            public object GetWishlist(int userId)
            {
                return _context.Wishlist
                    .Where(w => w.UserId == userId)
                    .Select(w => new
                    {
                        w.CarId,
                        CarName = $"{w.Car.Make} {w.Car.Model}",
                        Price = w.Car.price
                    })
                    .ToList();
            }

            public string RemoveFromWishlist(int userId, int CarId)
            {
                var item = _context.Wishlist
                    .FirstOrDefault(w => w.UserId == userId && w.CarId == CarId);

                if (item == null)
                    return "Item not found";

                _context.Wishlist.Remove(item);
                _context.SaveChanges();

                return "Removed from wishlist";
            }
        }
    }

