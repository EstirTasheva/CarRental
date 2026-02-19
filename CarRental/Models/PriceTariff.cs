using CarRental.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class PriceTariff
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Тип автомобил")]
        public CarType CarType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Цена на ден")]
        public decimal PricePerDay { get; set; }
    }
}
