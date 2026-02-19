using CarRental.Data;
using CarRental.Enums;
using CarRental.Models;
using CarRental.Models.ViewModels;
using CarRental.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class RentalContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEnumService<RentalContractStatus> _statusService;
        public RentalContractsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEnumService<RentalContractStatus> statusService)
        {
            _context = context;
            _userManager = userManager;
            _statusService = statusService;
        }

        [Authorize(Roles = "Employee,Administrator")]
        public async Task<IActionResult> Index(string? client, string? car, int? status, DateTime? fromDate, DateTime? toDate)
        {
            IQueryable<RentalContract> rental = _context.RentalContracts
                .Include(r => r.Car)
                .Include(r => r.Client)
                .OrderByDescending(r => r.StartDate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(client))
            {
                rental = rental.Where(r => (r.Client.FirstName + " " + r.Client.LastName).Contains(client) || r.Client.Email.Contains(client));
            }

            if (!string.IsNullOrWhiteSpace(car))
            {
                rental = rental.Where(r =>
                r.Car.Brand.Contains(car) ||
                r.Car.Model.Contains(car) ||
                r.Car.RegistrationNumber.Contains(car));
            }

            if (status.HasValue && status.Value != 0)
            {
                RentalContractStatus rentalStatus = (RentalContractStatus)status.Value;
                rental = rental.Where(r => r.Status == rentalStatus);
            }

            if (fromDate.HasValue)
            {
                rental = rental.Where(r => r.StartDate.Date >= fromDate.Value.Date);
            }

            if (toDate.HasValue)
            {
                rental = rental.Where(r => r.EndDate.Date <= toDate.Value.Date);
            }

            var viewModel = new RentalsIndexViewModel
            {
                Client = client,
                Car = car,
                Status = status,
                FromDate = fromDate,
                ToDate = toDate,
                Statuses = _statusService.GetAll(),
                Rentals = await rental.ToListAsync()
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(int carId)
        {
            Car? car = await _context.Cars.FindAsync(carId);

            if (car == null || car.Status == CarStatus.InService)
            {
                return NotFound();
            }

            RentalCreateViewModel model = new RentalCreateViewModel
            {
                CarId = carId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };

            ViewBag.Car = car;
            ViewBag.Tariffs = await _context.PriceTariffs.ToListAsync();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(RentalCreateViewModel model)
        {
            Car? car = await _context.Cars.FindAsync(model.CarId);

            if (car == null || car.Status == CarStatus.InService)
            {
                return NotFound();
            }
            ViewBag.Car = car;

            if (model.StartDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "Началната дата не може да е преди днешната дата.");
            }

            if (model.EndDate.Date < model.StartDate.Date)
            {
                ModelState.AddModelError(string.Empty, "Крайната дата не може да е преди началната дата.");
            }

            bool hasOverlap = await _context.RentalContracts.AnyAsync(r =>
                r.CarId == model.CarId &&
                r.Status == RentalContractStatus.Active &&
                model.StartDate.Date <= r.EndDate.Date &&
                model.EndDate.Date >= r.StartDate.Date);

            if (hasOverlap)
            {
                ModelState.AddModelError(string.Empty, "Автомобилът вече е нает за избрания период.");
            }

            if (car.Status == CarStatus.InService)
            {
                ModelState.AddModelError(string.Empty, "Автомобилът е в сервиз и не може да бъде нает.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            int days = (model.EndDate.Date - model.StartDate.Date).Days;

            if (days <= 0)
            {
                days = 1;
            }

            var tariff = await _context.PriceTariffs.FirstOrDefaultAsync(t => t.CarType == car.Type);

            if (tariff == null)
            {
                ModelState.AddModelError(string.Empty, "Няма зададена тарифа за този тип автомобил.");
                return RedirectToAction("Index", "Cars");
            }

            decimal total = days * tariff.PricePerDay;

            RentalContract contract = new RentalContract
            {
                CarId = car.Id,
                ClientId = userId,
                StartDate = model.StartDate.Date,
                EndDate = model.EndDate.Date,
                TotalPrice = total,
                Status = RentalContractStatus.Active
            };

            _context.RentalContracts.Add(contract);

            DateTime today = DateTime.Today;

            if (model.StartDate.Date <= today && model.EndDate.Date >= today)
            {
                car.Status = CarStatus.Rented;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Success");
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public IActionResult Success()
        {
            return View();
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MyRentals()
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            List<RentalContract> rentals = await _context.RentalContracts
                .Include(r => r.Car)
                .Where(r => r.ClientId == userId)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();

            return View(rentals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Employee,Administrator")]
        public async Task<IActionResult> Finish(int id)
        {
            RentalContract? contract = await _context.RentalContracts
                .Include(r => r.Car)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            if (contract.Status != RentalContractStatus.Active)
            {
                return RedirectToAction(nameof(Index));
            }

            contract.Status = RentalContractStatus.Finished;

            if (contract.Car.Status != CarStatus.InService)
            {
                contract.Car.Status = CarStatus.Available;
            }

            ApplicationUser? user = await _userManager.FindByIdAsync(contract.ClientId);
            if (user != null)
            {
                user.PreviousRentalsCount++;
                await _userManager.UpdateAsync(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Employee,Administrator")]
        public async Task<IActionResult> Cancel(int id)
        {
            RentalContract? contract = await _context.RentalContracts.Include(c => c.Car).FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            if (contract.Status != RentalContractStatus.Active)
            {
                return RedirectToAction(nameof(Index));
            }

            contract.Status = RentalContractStatus.Canceled;

            if (contract.Car.Status != CarStatus.InService)
            {
                contract.Car.Status = CarStatus.Available;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Employee,Administrator")]
        public async Task<IActionResult> Details(int id)
        {
            RentalContract? contract = await _context.RentalContracts
                .Include(r => r.Car)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }
    }
}
