using CarRental.Data;
using CarRental.Enums;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    public class PriceTariffsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PriceTariffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<PriceTariff> tariffs = await _context.PriceTariffs
                .OrderBy(t => t.CarType)
                .ToListAsync();

            return View(tariffs);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            PriceTariff? tariff = await _context.PriceTariffs.FindAsync(id);

            if (tariff == null)
            {
                return NotFound();
            }

            bool hasActiveRentalsForThisType = await _context.RentalContracts.AnyAsync(r =>
            r.Status == RentalContractStatus.Active && r.Car.Type == tariff.CarType);

            if (hasActiveRentalsForThisType)
            {
                TempData["Error"] = "Не може да редактирате тарифата, защото има активни наеми за този тип автомобили.";
                return RedirectToAction(nameof(Index));
            }

            return View(tariff);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PriceTariff model)
        {
            bool hasActiveRentalsForThisType = await _context.RentalContracts.AnyAsync(r =>
            r.Status == RentalContractStatus.Active && r.Car.Type == model.CarType);

            if (hasActiveRentalsForThisType)
            {
                TempData["Error"] = "Не може да редактирате тарифата, защото има активни наеми за този тип автомобили.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            PriceTariff? tariff = await _context.PriceTariffs.FindAsync(model.Id);

            if (tariff == null)
            {
                return NotFound();
            }

            tariff.PricePerDay = model.PricePerDay;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
