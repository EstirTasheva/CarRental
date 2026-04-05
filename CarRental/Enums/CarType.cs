using System.ComponentModel.DataAnnotations;

namespace CarRental.Enums
{
    public enum CarType
    {
        [Display(Name = "Седан")]
        Sedan = 1,

        [Display(Name = "SUV")]
        SUV = 2,

        [Display(Name = "Ван")]
        Van = 3,

        [Display(Name = "Хечбек")]
        Hatchback = 4,

        [Display(Name = "Купе")]
        Coupe = 5,

        [Display(Name = "Кабриолет")]
        Convertible = 6,

        [Display(Name = "Пикап")]
        Pickup = 7,

        [Display(Name = "Комби")]
        Wagon = 8
    }
}