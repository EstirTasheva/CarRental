using CarRental.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Полето „Рег. номер“ е задължително.")]
        [MaxLength(20, ErrorMessage = "Полето „Рег. номер“ може да е най-много 20 символа.")]
        [Display(Name = "Рег. номер")]
        public string RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Полето „Марка“ е задължително.")]
        [MaxLength(50, ErrorMessage = "Полето „Марка“ може да е най-много 50 символа.")]
        [Display(Name = "Марка")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Полето „Модел“ е задължително.")]
        [MaxLength(50, ErrorMessage = "Полето „Модел“ може да е най-много 50 символа.")]
        [Display(Name = "Модел")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Избери тип автомобил.")]
        [Display(Name = "Тип")]
        public CarType Type { get; set; }

        [Required(ErrorMessage = "Полето „Година“ е задължително.")]
        [Range(1900, 2100, ErrorMessage = "Годината трябва да е между 1900 и 2100")]
        [Display(Name = "Година")]
        public int Year { get; set; }

        [Display(Name = "Статус")]
        public CarStatus Status { get; set; }

        [Display(Name = "Снимка")]
        [Url(ErrorMessage = "Моля въведете валиден линк.")]
        public string? ImageUrl { get; set; }

        public int? PriceTariffId { get; set; }

        public PriceTariff? PriceTariff { get; set; }

        public ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
    }
}
