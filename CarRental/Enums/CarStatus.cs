using System.ComponentModel.DataAnnotations;

namespace CarRental.Enums
{
    public enum CarStatus
    {
        [Display(Name = "Наличен")]
        Available = 1,

        [Display(Name = "Нает")]
        Rented = 2,

        [Display(Name = "В сервиз")]
        InService = 3
    }
}