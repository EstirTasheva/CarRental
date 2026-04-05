using System.ComponentModel.DataAnnotations;

namespace CarRental.Enums
{
    // Роли в системата - администратор, служител и клиент
    public enum Role
    {
        [Display(Name = "Администратор")]
        Administrator,

        [Display(Name = "Служител")]
        Employee,

        [Display(Name = "Клиент")]
        Client
    }
}