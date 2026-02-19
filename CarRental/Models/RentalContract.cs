using CarRental.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental.Models
{
    public class RentalContract
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public string ClientId { get; set; }
        public ApplicationUser Client { get; set; }

        [Required]
        [Display(Name = "Начална дата")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Крайна дата")]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Обща цена")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Статус")]
        public RentalContractStatus Status { get; set; }
    }
}
