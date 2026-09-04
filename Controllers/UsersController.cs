using Microsoft.AspNetCore.Mvc;
using Online_Car_Marketplace.Models;
using Microsoft.EntityFrameworkCore;
using Online_Car_Marketplace.Data;

namespace Online_Car_Marketplace.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Users/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Optional: Check if username/email already exists
                var exists = await _context.Users
                    .AnyAsync(u => u.Username == user.Username || u.Email == user.Email);
                if (exists)
                {
                    ModelState.AddModelError("", "Username or Email already exists!");
                    return View(user);
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Users/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Store user info in session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                return RedirectToAction("Index", "Cars");
            }

            ModelState.AddModelError("", "Invalid Username or Password");
            return View();
        }
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }
        // GET: Users/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Remove session
            return RedirectToAction("Login");
        }
    }
}
