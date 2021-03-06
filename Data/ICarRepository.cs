using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PRAPI.Models;

namespace PRAPI.Data
{
    public interface ICarRepository
    {
        bool CreateCar(Car car);
        Task<Car> GetCar(int id);
        Task<List<Car>> GetAllCars();
        Task<List<Car>> GetAllCarModels();
        Task<List<Car>> SearchForCars(SearchParams searchParams);
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
    }
}