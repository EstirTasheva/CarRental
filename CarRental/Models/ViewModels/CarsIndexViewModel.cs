namespace CarRental.Models.ViewModels
{
    // ViewModel за предаване на данни към изгледа за списък с коли, включително филтри и тарифи
    public class CarsIndexViewModel
    {
        public List<Car> Cars { get; set; } = new();
        public string? Brand { get; set; }
        public int? Status { get; set; }
        public int? Type { get; set; }
        public int? Year { get; set; }

        public IEnumerable<EnumDTO> Statuses { get; set; } = new List<EnumDTO>();
        public IEnumerable<EnumDTO> Types { get; set; } = new List<EnumDTO>();

        public List<PriceTariff> Tariffs { get; set; } = new();
    }
}
