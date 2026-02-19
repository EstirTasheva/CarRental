using CarRental.Enums;
using CarRental.Models;

namespace CarRental.Data.Seed
{
    public class PriceTariffSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.PriceTariffs.Any())
            {
                List<PriceTariff> priceTariffs = new List<PriceTariff>
                {
                    new PriceTariff { CarType = CarType.Sedan, PricePerDay = 50 },
                    new PriceTariff { CarType = CarType.Coupe, PricePerDay = 60 },
                    new PriceTariff { CarType = CarType.Van, PricePerDay = 50 },
                    new PriceTariff { CarType = CarType.Hatchback, PricePerDay = 45 },
                    new PriceTariff { CarType = CarType.Convertible, PricePerDay = 90 },
                    new PriceTariff { CarType = CarType.Wagon, PricePerDay = 60 },
                    new PriceTariff { CarType = CarType.SUV, PricePerDay = 80 },
                    new PriceTariff { CarType = CarType.Pickup, PricePerDay = 120 }
                };
                context.PriceTariffs.AddRange(priceTariffs);
                context.SaveChanges();
            }
        }
    }
}
