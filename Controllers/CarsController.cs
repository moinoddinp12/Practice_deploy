using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Car_Marketplace.Data;
using Online_Car_Marketplace.Models;

namespace Online_Car_Marketplace.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsUserLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string searchMake, string searchCity, decimal? minPrice, decimal? maxPrice, string sortOrder)
        {
            // Sorting state values for links in the view
            ViewData["MakeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "make_desc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
            ViewData["YearSortParm"] = sortOrder == "Year" ? "year_desc" : "Year";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            var cars = from c in _context.Cars
                       select c;

            // --- Filtering ---
            if (!string.IsNullOrEmpty(searchMake))
            {
                cars = cars.Where(c => c.Make.Contains(searchMake));
            }

            if (!string.IsNullOrEmpty(searchCity))
            {
                cars = cars.Where(c => c.City.Contains(searchCity));
            }

            if (minPrice.HasValue)
            {
                cars = cars.Where(c => c.price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                cars = cars.Where(c => c.price <= maxPrice.Value);
            }

            // --- Sorting ---
            switch (sortOrder)
            {
                case "make_desc":
                    cars = cars.OrderByDescending(c => c.Make);
                    break;
                case "Price":
                    cars = cars.OrderBy(c => c.price);
                    break;
                case "price_desc":
                    cars = cars.OrderByDescending(c => c.price);
                    break;
                case "Year":
                    cars = cars.OrderBy(c => c.year);
                    break;
                case "year_desc":
                    cars = cars.OrderByDescending(c => c.year);
                    break;
                case "Date":
                    cars = cars.OrderBy(c => c.CreatedAt);
                    break;
                case "date_desc":
                    cars = cars.OrderByDescending(c => c.CreatedAt);
                    break;
                default:
                    cars = cars.OrderBy(c => c.Make);
                    break;
            }

            return View(await cars.AsNoTracking().ToListAsync());
        }


        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            if (car == null) return NotFound();

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {

            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Users");

            if (HttpContext.Session.GetString("Role") != "Admin") // ✅ inline check
                return RedirectToAction("Index", "Cars");

            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Car car, IFormFile? CarImageFile)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Users");

            if (HttpContext.Session.GetString("Role") != "Admin") // ✅ inline check
                return RedirectToAction("Index", "Cars");

            if (ModelState.IsValid)
            {
                if (CarImageFile != null && CarImageFile.Length > 0)
                {
                    string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cars");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                    string fileName = Guid.NewGuid() + Path.GetExtension(CarImageFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await CarImageFile.CopyToAsync(stream);
                    }

                    car.ImagePath = fileName;
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Users");

            if (HttpContext.Session.GetString("Role") != "Admin") // ✅ inline check
                return RedirectToAction("Index", "Cars");

            if (id == null) return NotFound();

            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car form, IFormFile? CarImageFile)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Users");

            if (HttpContext.Session.GetString("Role") != "Admin") // ✅ inline check
                return RedirectToAction("Index", "Cars");

            if (id != form.Id) return NotFound();
            if (!ModelState.IsValid) return View(form);

            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            // Update fields
            car.Make = form.Make;
            car.Model = form.Model;
            car.year = form.year;
            car.Mileage = form.Mileage;
            car.price = form.price;
            car.Description = form.Description;
            car.City = form.City;
            car.IsActive = form.IsActive;
            car.UpdatedAt = DateTime.UtcNow;

            // Handle new image upload
            if (CarImageFile != null && CarImageFile.Length > 0)
            {
                string dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cars");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                string fileName = Guid.NewGuid() + Path.GetExtension(CarImageFile.FileName);
                string newPath = Path.Combine(dir, fileName);
                using (var stream = new FileStream(newPath, FileMode.Create))
                    await CarImageFile.CopyToAsync(stream);

                // Delete old image if exists
                if (!string.IsNullOrEmpty(car.ImagePath))
                {
                    string oldPath = Path.Combine(dir, car.ImagePath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                car.ImagePath = fileName;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Cars.Any(e => e.Id == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Admin"; // ✅ only true if Role = Admin
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Users");

            if (HttpContext.Session.GetString("Role") != "Admin") // ✅ inline check
                return RedirectToAction("Index", "Cars");

            if (id == null) return NotFound();

            var car = await _context.Cars.FirstOrDefaultAsync(m => m.Id == id);
            if (car == null) return NotFound();

            return View(car);
        }



        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Users");

            if (HttpContext.Session.GetString("Role") != "Admin") // ✅ inline check
                return RedirectToAction("Index", "Cars");

            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(car.ImagePath))
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cars", car.ImagePath);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }

                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}
