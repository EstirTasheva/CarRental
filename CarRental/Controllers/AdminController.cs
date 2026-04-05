using CarRental.Data;
using CarRental.Enums;
using CarRental.Models;
using CarRental.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    // Достъп само за администратора
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // Списък с потребители
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.Email)
                .ToListAsync();

            return View(users);
        }

        // Променя ролята на потребител на служител
        [HttpPost]
        public async Task<IActionResult> MakeEmployee(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            bool hasActiveRentals = await _context.RentalContracts
                .AnyAsync(r => r.ClientId == user.Id && r.Status == RentalContractStatus.Active);

            if (hasActiveRentals)
            {
                TempData["Error"] = "Потребител с активен договор не може да бъде променен в служител.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.RemoveFromRoleAsync(user, Role.Client.ToString());

            if (!await _userManager.IsInRoleAsync(user, Role.Employee.ToString()))
            {
                await _userManager.AddToRoleAsync(user, Role.Employee.ToString());
            }

            return RedirectToAction(nameof(Index));
        }

        // Премахва ролята на служител и връща потребителя като клиент
        [HttpPost]
        public async Task<IActionResult> RemoveEmployee(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            await _userManager.RemoveFromRoleAsync(user, Role.Employee.ToString());

            if (!await _userManager.IsInRoleAsync(user, Role.Client.ToString()))
            {
                await _userManager.AddToRoleAsync(user, Role.Client.ToString());
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult CreateClient()
        {
            return View();
        }

        // Създава нов клиент
        [HttpPost]
        public async Task<IActionResult> CreateClient(CreateClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PreviousRentalsCount = 0
            };

            ApplicationUser? existing = await _userManager.FindByEmailAsync(model.Email);

            if (existing != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Този имейл вече е регистриран.");
                return View(model);
            }

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Потребителят не можа да бъде създаден.");
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, Role.Client.ToString());

            if (!roleResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Ролята не можа да бъде добавена.");
                await _userManager.DeleteAsync(user);
                return View(model);
            }

            return RedirectToAction(nameof(Clients));
        }

        // Зарежда формата за редакция на клиент
        [HttpGet]
        public async Task<IActionResult> EditClient(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            EditClientViewModel model = new EditClientViewModel
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        // Запазва промените по избран клиент
        [HttpPost]
        public async Task<IActionResult> EditClient(EditClientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser? user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            ApplicationUser? existing = await _userManager.FindByEmailAsync(model.Email);

            if (existing != null && existing.Id != user.Id)
            {
                ModelState.AddModelError(nameof(model.Email), "Този имейл вече е регистриран.");
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Възникна грешка при редактирането на клиента.");
                return View(model);
            }

            return RedirectToAction(nameof(Clients));
        }

        // Изтрива клиент, ако няма свързани договори
        [HttpPost]
        public async Task<IActionResult> DeleteClient(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            bool hasContracts = await _context.RentalContracts.AnyAsync(r => r.ClientId == userId);

            if (hasContracts)
            {
                TempData["Error"] = "Не може да изтриете този клиент, тъй като има свързани договори за наем.";
                return RedirectToAction(nameof(Clients));
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Възникна грешка при изтриването на клиента.";
                return RedirectToAction(nameof(Clients));
            }

            return RedirectToAction(nameof(Clients));
        }

        // Търсене на клиенти
        [HttpGet]
        public async Task<IActionResult> Clients(string search)
        {
            IList<ApplicationUser> clients = await _userManager.GetUsersInRoleAsync(Role.Client.ToString());

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                string searchText = search.ToLower();

                clients = clients
                    .Where(c => (c.FirstName + " " + c.LastName).ToLower().Contains(searchText) ||
                                (c.Email ?? "").ToLower().Contains(searchText) ||
                                (c.PhoneNumber ?? "").ToLower().Contains(searchText))
                    .ToList();
            }
            else
            {
                clients = clients.OrderBy(c => c.Email).ToList();
            }

            ViewBag.Search = search ?? string.Empty;
            return View(clients);
        }

        // Показва клиентите с най-много предишни наеми
        [HttpGet]
        public async Task<IActionResult> TopClients(int top = 5)
        {
            IList<ApplicationUser> clients = await _userManager
                .GetUsersInRoleAsync(Role.Client.ToString()); 
            
            var topClients = clients
                .OrderByDescending(u => u.PreviousRentalsCount)
                .Take(top)
                .ToList();

            return View(topClients);
        }
    }
}