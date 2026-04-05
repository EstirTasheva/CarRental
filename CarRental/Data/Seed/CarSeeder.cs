using CarRental.Enums;
using CarRental.Models;

namespace CarRental.Data.Seed
{
    public class CarSeeder
    {
        // Добавя начални автомобили в базата данни, ако все още не съществуват
        public static void SeedCars(ApplicationDbContext context)
        {
            if (!context.Cars.Any())
            {
                List<Car> cars = new List<Car>
                {
                    new Car 
                    { 
                        RegistrationNumber = "CB1234AB", 
                        Brand = "Toyota", 
                        Model = "Camry", 
                        Year = 2020, 
                        Type = CarType.Sedan, 
                        Status = CarStatus.Available, 
                        PriceTariffId = 1,
                        ImageUrl = "https://i.postimg.cc/8kwTnyGk/Toyota_Camry.png" 
                    },
                    new Car 
                    { 
                        RegistrationNumber = "PB9090PP", 
                        Brand = "Ford", 
                        Model = "Mustang", 
                        Year = 2021, 
                        Type = CarType.Convertible, 
                        Status = CarStatus.Available, 
                        PriceTariffId = 5, 
                        ImageUrl = "https://i.postimg.cc/qBQpZmrz/Ford_Mustang.png" 
                    },
                    new Car 
                    { 
                        RegistrationNumber = "CA5678CC", 
                        Brand = "Audi", 
                        Model = "Q5", 
                        Year = 2019, 
                        Type = CarType.SUV, 
                        Status = CarStatus.Available, 
                        PriceTariffId = 7,
                        ImageUrl = "https://i.postimg.cc/HW3TPSmJ/Audi_Q5.png" 
                    }
                };
                context.Cars.AddRange(cars);
                context.SaveChanges();
            }
        }
    }
}
