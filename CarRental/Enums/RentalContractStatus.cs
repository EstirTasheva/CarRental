using System.ComponentModel.DataAnnotations;

namespace CarRental.Enums
{
    public enum RentalContractStatus
    {
        [Display(Name = "Активен")]
        Active = 1,

        [Display(Name = "Завършен")]
        Finished = 2,

        [Display(Name = "Анулиран")]
        Canceled = 3
    }
}