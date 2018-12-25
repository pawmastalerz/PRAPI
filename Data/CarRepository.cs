using System;
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

        public async Task<List<Car>> GetAllCars()
        {
            return await this.context.Cars
                .OrderBy(c => c.Model)
                .ToListAsync();
        }

        public async Task<List<Car>> GetAllCarModels()
        {
            return await this.context.Cars
                .GroupBy(c => c.Model)
                .Select(g => g.FirstOrDefault())
                .ToListAsync();
        }

        public async Task<List<Car>> SearchForCarsForUser(SearchParams searchParams)
        {
            var sql = from c in this.context.Cars
                      where (c.Model == searchParams.Model) && (
                          !this.context.Orders.Any(o => (
                              (o.CarId == c.CarId) && (
                                ((o.ReservedFrom < searchParams.ReservedFrom) &&
                                (o.ReservedTo >= searchParams.ReservedFrom)) ||
                                ((o.ReservedFrom <= searchParams.ReservedTo) &&
                                (o.ReservedTo > searchParams.ReservedTo)) ||
                                ((o.ReservedFrom >= searchParams.ReservedFrom) &&
                                (o.ReservedTo <= searchParams.ReservedTo))
                              )
                          ))
                      )
                      orderby c.Model
                      select c;

            return await sql.ToListAsync();
        }

        public async Task<Car> GetCar(int id)
        {
            var carToReturn = await this.context.Cars
                .FirstOrDefaultAsync(p => p.CarId == id);
            return carToReturn;
        }

        public void Delete<T>(T entity) where T : class
        {
            this.context.Remove(entity);
        }

        public async Task<bool> SaveAll()
        {
            var saved = await this.context.SaveChangesAsync() > 0;
            return saved;
        }
    }
}