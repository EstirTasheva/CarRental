using CarRental.Enums;
using CarRental.Models;

namespace CarRental.Data.Seed
{
    public class CarSeeder
    {
        public static void SeedCars(ApplicationDbContext context)
        {
            if (!context.Cars.Any())
            {
                List<Car> cars = new List<Car>
                {
                    new Car { RegistrationNumber = "CB1234AB", Brand = "Toyota", Model = "Camry", Year = 2020, Type = CarType.Sedan, Status = CarStatus.Available, PriceTariffId = 1,ImageUrl = "https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" },
                    new Car { RegistrationNumber = "PB9090PP", Brand = "Ford", Model = "Mustang", Year = 2021, Type = CarType.Convertible, Status = CarStatus.Available, PriceTariffId = 5, ImageUrl = "https://images.unsplash.com/photo-1670827061518-627830643239?q=80&w=1170&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" },
                    new Car { RegistrationNumber = "CA5678CC", Brand = "Audi", Model = "Q5", Year = 2019, Type = CarType.SUV, Status = CarStatus.Available, PriceTariffId = 7,ImageUrl = "https://images.unsplash.com/photo-1769641156659-9eb1301282d2?q=80&w=880&auto=format&fit=crop&ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" }
                };
                context.Cars.AddRange(cars);
                context.SaveChanges();
            }
        }
    }
}
