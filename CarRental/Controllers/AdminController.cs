using CarRental.Enums;
using CarRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Controllers
{
    [Authorize(Roles = nameof(Role.Administrator))]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

            // Вземаме всички роли на потребителя
            var roles = await _userManager.GetRolesAsync(user);

            // Махаме Employee, ако го има
            if (roles.Contains(Role.Employee.ToString()))
            {
                await _userManager.RemoveFromRoleAsync(user, Role.Employee.ToString());
            }

            // Ако няма никаква роля, връщаме Client
            roles = await _userManager.GetRolesAsync(user);

            if (!roles.Any())
            {
                await _userManager.AddToRoleAsync(user, Role.Client.ToString());
            }

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> TopCLients(int top = 5)
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
