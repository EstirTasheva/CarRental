using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels
{
    public class RentalCreateViewModel
    {
        public int CarId { get; set; }

        [Required(ErrorMessage = "Изберете начална дата.")]
        [Display(Name = "Начална дата")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Изберете крайна дата.")]
        [Display(Name = "Крайна дата")]
        public DateTime EndDate { get; set; }
    }
}
