using Microsoft.AspNetCore.Mvc;
using Online_Car_Marketplace.Services.Interfaces;

namespace Online_Car_Marketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _service;

        public WishlistController(IWishlistService service)
        {
            _service = service;
        }

        // ✅ Add to Wishlist
        [HttpPost("{carId}")]
        public IActionResult Add(int carId)
        {
            int userId = 1; // temporary

            var result = _service.AddToWishlist(userId, carId);
            return Ok(result);
        }

        // ✅ Get Wishlist
        [HttpGet]
        public IActionResult Get()
        {
            int userId = 1;

            var data = _service.GetWishlist(userId);
            return Ok(data);
        }

        // ✅ Remove from Wishlist
        [HttpDelete("{carId}")]
        public IActionResult Remove(int carId)
        {
            int userId = 1;

            var result = _service.RemoveFromWishlist(userId, carId);
            return Ok(result);
        }
    }
}