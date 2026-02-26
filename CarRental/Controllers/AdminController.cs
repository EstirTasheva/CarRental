using CarRental.Data;
using CarRental.Enums;
using CarRental.Models;
using CarRental.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    [Authorize(Roles = nameof(Role.Administrator))]
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

        // Направи служител
        [HttpPost]
        public async Task<IActionResult> MakeEmployee(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Ако е Client -> махаме Client
            if (await _userManager.IsInRoleAsync(user, Role.Client.ToString()))
            {
                await _userManager.RemoveFromRoleAsync(user, Role.Client.ToString());
            }

            await _userManager.AddToRoleAsync(user, Role.Employee.ToString());

            return RedirectToAction(nameof(Index));
        }

        // Махни служител (връща го на Client)
        [HttpPost]
        public async Task<IActionResult> RemoveEmployee(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains(Role.Employee.ToString()))
            {
                await _userManager.RemoveFromRoleAsync(user, Role.Employee.ToString());
            }

            roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any())
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

            await _userManager.CreateAsync(user, model.Password);

            await _userManager.AddToRoleAsync(user, Role.Client.ToString());
            return RedirectToAction(nameof(Clients));
        }


        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            ApplicationUser? existing = await _userManager.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Този имейл вече е регистриран.");
                return View(model);
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Clients));
        }

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

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Clients));
        }

        // Търсене на клиенти
        [HttpGet]
        public async Task<IActionResult> Clients(string search)
        {
            IList<ApplicationUser> clients = await _userManager.GetUsersInRoleAsync(Role.Client.ToString());

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                clients = clients
                    .Where(c => (c.FirstName + " " + c.LastName).ToLower().Contains(search) ||
                                (c.Email ?? "").ToLower().Contains(search) || (c.PhoneNumber ?? "").ToLower().Contains(search))
                    .ToList();
            }
            else
            {
                clients = clients.OrderBy(c => c.Email).ToList();
            }

            ViewBag.Search = search ?? string.Empty;
            return View(clients);
        }

        [HttpGet]
        public async Task<IActionResult> TopClients(int top = 5)
        {
            IList<ApplicationUser> clients = await _userManager.GetUsersInRoleAsync(Role.Client.ToString());
            var topClients = clients
                .OrderByDescending(u => u.PreviousRentalsCount)
                .Take(top)
                .ToList();

            return View(topClients);
        }
    }
}
