using CarRental.Data;
using CarRental.Data.Seed;
using CarRental.Enums;
using CarRental.Models;
using CarRental.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEnumService<CarStatus>, EnumService<CarStatus>>();
builder.Services.AddTransient<IEnumService<CarType>, EnumService<CarType>>();
builder.Services.AddTransient<IEnumService<RentalContractStatus>, EnumService<RentalContractStatus>>();

var app = builder.Build();

using (IServiceScope service = app.Services.CreateScope())
{
    IServiceProvider provider = service.ServiceProvider;

    RoleManager<IdentityRole> roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeeder.SeedRolesAsync(roleManager);

    UserManager<ApplicationUser> userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
    await IdentitySeeder.SeedAdminAsync(userManager);
    await IdentitySeeder.SeedEmployeeAsync(userManager);
    await IdentitySeeder.SeedClientAsync(userManager);

    ApplicationDbContext dbContext = provider.GetRequiredService<ApplicationDbContext>();
    PriceTariffSeeder.Seed(dbContext);
    CarSeeder.SeedCars(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
