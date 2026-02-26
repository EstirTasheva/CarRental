using System.ComponentModel.DataAnnotations;

namespace CarRental.Models.ViewModels
{
    public class CreateClientViewModel
    {
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

        [Required(ErrorMessage = "Полето „Парола“ е задължително.")]
        [StringLength(100, ErrorMessage = "Паролата трябва да е минимум 6 символа.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[^a-zA-Z0-9]).+$", ErrorMessage = "Паролата трябва да съдържа поне един специален символ.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Полето „Потвърди паролата“ е задължително.")]
        [DataType(DataType.Password)]
        [Display(Name = "Потвърди паролата")]
        [Compare("Password", ErrorMessage = "Паролата и потвърждението на паролата не съвпадат.")]
        public string ConfirmPassword { get; set; }
    }
}
