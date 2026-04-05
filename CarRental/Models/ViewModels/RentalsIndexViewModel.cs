namespace CarRental.Models.ViewModels
{
    // ViewModel за предаване на данни към изгледа за списък с наеми, включително филтри и статуси
    public class RentalsIndexViewModel
    {
        public List<RentalContract> Rentals { get; set; } = new();
        public string? Client { get; set; }
        public int? CarId { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public IEnumerable<EnumDTO> Statuses { get; set; } = new List<EnumDTO>();

        public List<Car> Cars { get; set; } = new();
    }
}
