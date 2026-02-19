using Microsoft.AspNetCore.Identity;

namespace CarRental.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int PreviousRentalsCount { get; set; } = 0;

        public ICollection<RentalContract> RentalContracts { get; set; }
    }
}
