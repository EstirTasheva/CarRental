using CarRental.Models;

namespace CarRental.Services
{
    // Интерфейс за EnumService, който дефинира метода GetAll()
    public interface IEnumService<T>
    {
        IEnumerable<EnumDTO> GetAll();
    }
}
