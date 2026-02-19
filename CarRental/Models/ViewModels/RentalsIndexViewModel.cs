namespace CarRental.Models.ViewModels
{
    public class RentalsIndexViewModel
    {
        public List<RentalContract> Rentals { get; set; } = new();
        public string? Client { get; set; }
        public string? Car { get; set; }
        public int? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public IEnumerable<EnumDTO> Statuses { get; set; } = new List<EnumDTO>();
    }
}
