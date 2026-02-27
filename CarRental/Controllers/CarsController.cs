using CarRental.Data;
using CarRental.Enums;
using CarRental.Models;
using CarRental.Models.ViewModels;
using CarRental.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class CarsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEnumService<CarStatus> _statusService;
        private readonly IEnumService<CarType> _typeService;

        public CarsController(ApplicationDbContext context, IEnumService<CarStatus> statusService, IEnumService<CarType> typeService)
        {
            _context = context;
            _statusService = statusService;
            _typeService = typeService;
        }

        //Списък + филтри
        public async Task<IActionResult> Index(string? brand, int? status, int? type, int? year)
        {
            IQueryable<Car> cars = _context.Cars.Include(c => c.PriceTariff).AsQueryable();

            if (!string.IsNullOrWhiteSpace(brand))
            {
                cars = cars.Where(car => car.Brand.Contains(brand));
            }

            if (status.HasValue)
            {
                CarStatus carStatus = (CarStatus)status.Value;
                cars = cars.Where(car => car.Status == carStatus);
            }

            if (type.HasValue)
            {
                CarType carType = (CarType)type.Value;
                cars = cars.Where(car => car.Type == carType);
            }

            if (year.HasValue)
            {
                cars = cars.Where(car => car.Year == year.Value);
            }

            var viewModel = new CarsIndexViewModel
            {
                Brand = brand,
                Status = status,
                Type = type,
                Year = year,
                Statuses = _statusService.GetAll(),
                Types = _typeService.GetAll(),
                Tariffs = await _context.PriceTariffs.ToListAsync(),
                Cars = await cars.OrderBy(car => car.Brand).ThenBy(car => car.PriceTariff.PricePerDay).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewBag.Types = _typeService.GetAll();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(Car car)
        {
            car.Status = CarStatus.Available;

            PriceTariff? tariff = await _context.PriceTariffs
                .FirstOrDefaultAsync(t => t.CarType == car.Type);

            if (tariff == null)
            {
                ModelState.AddModelError(nameof(car.Type), "Няма зададена тарифа за избрания тип автомобил.");
            }
            else
            {
                car.PriceTariffId = tariff.Id;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Types = _typeService.GetAll();
                return View(car);
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id)
        {
            Car car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            ViewBag.Types = _typeService.GetAll();
            ViewBag.Tariffs = await _context.PriceTariffs.ToListAsync();

            return View(car);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            Car? existing = await _context.Cars.FindAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            PriceTariff? tariff = await _context.PriceTariffs.FirstOrDefaultAsync(t => t.CarType == car.Type);

            if (tariff == null)
            {
                ModelState.AddModelError(string.Empty, "Няма зададена тарифа за избрания тип автомобил.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Types = _typeService.GetAll();
                car.Status = existing.Status;
                car.PriceTariffId = existing.PriceTariffId;

                return View(car);
            }

            existing.RegistrationNumber = car.RegistrationNumber;
            existing.Brand = car.Brand;
            existing.Model = car.Model;
            existing.Year = car.Year;
            existing.Type = car.Type;
            existing.PriceTariffId = tariff.Id;
            existing.ImageUrl = car.ImageUrl;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            Car? car = await _context.Cars
                .Include(c => c.PriceTariff)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            Car? car = await _context.Cars
                 .Include(c => c.PriceTariff)
                 .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Car? car = await _context.Cars
                .Include(c => c.RentalContracts)
                .Include(c => c.PriceTariff)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            if (car.RentalContracts.Any())
            {
                TempData["Error"] = "Автомобилът не може да бъде изтрит, защото е свързан с договори.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Administrator")]
        public async Task<IActionResult> SendToService(int id)
        {
            Car? car = await _context.Cars.FindAsync(id);

            if (car == null)
            {
                return NotFound();
            }

            car.Status = CarStatus.InService;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ReturnFromService(int id)
        {
            Car? car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            car.Status = CarStatus.Available;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Employee,Administrator")]
        public async Task<IActionResult> GetPriceForType(CarType type)
        {
            var tariff = await _context.PriceTariffs.FirstOrDefaultAsync(t => t.CarType == type);

            if (tariff == null)
            {
                return NotFound();
            }

            return Json(tariff.PricePerDay);
        }
    }
}

