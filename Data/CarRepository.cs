using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PRAPI.Models;

namespace PRAPI.Data
{
    public class CarRepository : ICarRepository
    {
        private readonly DataContext context;
        private readonly IHostingEnvironment hostingEnvironment;

        public CarRepository(DataContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }
        public bool CreateCar(Car car)
        {
            this.context.Cars.Add(car);
            this.context.SaveChanges();
            return true;
        }

        public void Delete<T>(T entity) where T : class
        {
            this.context.Remove(entity);
        }

        public void DeleteFile(Car car)
        {
            
        }

        public async Task<List<Car>> GetAllCars()
        {
            return await this.context.Cars
                .FromSql("SELECT * FROM prorental.Cars")
                .ToListAsync();
        }

        public async Task<List<Car>> GetNewsCars()
        {
            return await this.context.Cars
                .FromSql("SELECT * FROM prorental.Cars WHERE Accepted = 1")
                .ToListAsync();
        }

        public async Task<Car> GetCar(int id)
        {
            var carToReturn = await this.context.Cars
                .FirstOrDefaultAsync(p => p.Id == id);
            return carToReturn;
        }

        public async Task<bool> SaveAll()
        {
            var saved = await this.context.SaveChangesAsync() > 0;
            return saved;
        }
    }
}