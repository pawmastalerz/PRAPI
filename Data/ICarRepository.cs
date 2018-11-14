using System.Collections.Generic;
using System.Threading.Tasks;
using PRAPI.Models;

namespace PRAPI.Data
{
    public interface ICarRepository
    {
        bool CreateCar(Car car);
        Task<Car> GetCar(int id);
        Task<List<Car>> GetAllCars();
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
    }
}