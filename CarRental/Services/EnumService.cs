using CarRental.Models;
using CarRental.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CarRental.Services
{
    public class EnumService<T> : IEnumService<T> where T : Enum
    {
        // Метод за извличане на всички стойности от enum типа T и тяхното дисплей име
        public IEnumerable<EnumDTO> GetAll()
        {
            IEnumerable<T>? enumValues = Enum.GetValues(typeof(T)) as IEnumerable<T>;

            IEnumerable<EnumDTO> enumDTOs = enumValues.Select(enumValue => new EnumDTO
            {
                Value = Convert.ToInt32(enumValue),
                Text = enumValue.GetDisplayName()
            });

            return enumDTOs;
        }
    }
}
