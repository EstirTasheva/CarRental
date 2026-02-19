using CarRental.Models;

namespace CarRental.Services
{
    public interface IEnumService<T>
    {
        IEnumerable<EnumDTO> GetAll();
    }
}
