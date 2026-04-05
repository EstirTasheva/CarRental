using System.ComponentModel.DataAnnotations;

namespace CarRental.Enums
{
    // Статус на договор за наем - активен, завършен или анулиран
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