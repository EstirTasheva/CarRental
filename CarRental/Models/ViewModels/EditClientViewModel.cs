using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels
{
    public class EditClientViewModel
    {
        public string UserId { get; set; } = "";

        [Required(ErrorMessage = "Полето „Име“ е задължително.")]
        [Display(Name = "Име")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Полето „Фамилия“ е задължително.")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = "";

        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Телефонният номер трябва да съдържа точно 10 цифри и да започва с 0.")]
        [Display(Name = "Телефонен номер")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Полето „Имейл“ е задължително.")]
        [EmailAddress(ErrorMessage = "Въведете валиден имейл адрес.")]
        [Display(Name = "Имейл")]
        public string Email { get; set; }
    }
}
