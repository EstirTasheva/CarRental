using CarRental.Models;
using Microsoft.AspNetCore.Identity;
using CarRental.Enums;

namespace CarRental.Data.Seed
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (string roleName in Enum.GetNames(typeof(Role)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser adminUser = new()
            {
                UserName = "admin@carrental.com",
                Email = "admin@carrental.com",
                FirstName = "Администратор",
                LastName = "Система",
                EmailConfirmed = true
            };

            var existingUser = await userManager.FindByEmailAsync(adminUser.Email);

            if (existingUser == null)
            {
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, Role.Administrator.ToString());
            }
        }

        public static async Task SeedEmployeeAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser employeeUser = new()
            {
                UserName = "employee@carrental.com",
                Email = "employee@carrental.com",
                FirstName = "Иван",
                LastName = "Иванов",
                EmailConfirmed = true
            };

            var existingUser = await userManager.FindByEmailAsync(employeeUser.Email);

            if (existingUser == null)
            {
                await userManager.CreateAsync(employeeUser, "Employee123!");
                await userManager.AddToRoleAsync(employeeUser, Role.Employee.ToString());
            }
        }

        public static async Task SeedClientAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser clientUser = new()
            {
                UserName = "client@carrental.com",
                Email = "client@carrental.com",
                FirstName = "Естир",
                LastName = "Ташева",
                EmailConfirmed = true
            };

            var existingUser = await userManager.FindByEmailAsync(clientUser.Email);

            if (existingUser == null)
            {
                await userManager.CreateAsync(clientUser, "Client123!");
                await userManager.AddToRoleAsync(clientUser, Role.Client.ToString());
            }
        }
    }
}
