using Microsoft.AspNetCore.Mvc;
using Online_Car_Marketplace.Data;
using Online_Car_Marketplace.Models;

namespace Online_Car_Marketplace.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new AdminDashboardViewModel
            {
                TotalCars = _context.Cars.Count(),
                TotalUsers = _context.Users.Count()
            };

            return View(model);
        }

    }
}

