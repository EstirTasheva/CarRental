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
                    new Car { RegistrationNumber = "CB1234AB", Brand = "Toyota", Model = "Camry", Year = 2020, Type = CarType.Sedan, Status = CarStatus.Available, PriceTariffId = 1,ImageUrl = "/images/cars/ToyotaCamry.png" },
                    new Car { RegistrationNumber = "PB9090PP", Brand = "Ford", Model = "Mustang", Year = 2021, Type = CarType.Convertible, Status = CarStatus.Available, PriceTariffId = 5, ImageUrl = "/images/cars/FordMustang.png" },
                    new Car { RegistrationNumber = "CA5678CC", Brand = "Audi", Model = "Q5", Year = 2019, Type = CarType.SUV, Status = CarStatus.Available, PriceTariffId = 7,ImageUrl = "/images/cars/AudiQ5.png" }
                };
                context.Cars.AddRange(cars);
                context.SaveChanges();
            }
        }
    }
}
