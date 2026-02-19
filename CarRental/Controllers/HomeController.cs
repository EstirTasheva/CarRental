using CarRental.Data;
using CarRental.Enums;
using CarRental.Models;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            var cars = await _context.Cars
                .Where(c => c.Status == CarStatus.Available)
                .OrderBy(c => c.Id)
                .Take(3)
                .ToListAsync();

            return View(cars);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
