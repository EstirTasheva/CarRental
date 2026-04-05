using System.ComponentModel.DataAnnotations;

namespace CarRental.Enums
{
    // Статус на колата - налична, наета или в сервиз
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