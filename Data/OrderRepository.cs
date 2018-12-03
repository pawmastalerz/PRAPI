using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PRAPI.Models;

namespace PRAPI.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext context;
        private readonly IHostingEnvironment hostingEnvironment;

        public OrderRepository(DataContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        public bool CreateOrder(Order order)
        {
            this.context.Orders.Add(order);
            this.context.SaveChanges();
            return true;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await this.context.Orders
                .OrderBy(o => o.OrderId)
                .ToListAsync();
        }

        public async Task<List<Order>> SearchForUserOrders(SearchParams searchParams)
        {
            return await this.context.Orders
                // .Where(c =>
                //     (c.Model == searchParams.Model) &&
                //     (
                //         (
                //             searchParams.ReservedFrom < c.ReservedFrom &&
                //             searchParams.ReservedTo < c.ReservedFrom
                //         ) ||
                //         (
                //         searchParams.ReservedFrom > c.ReservedTo &&
                //         searchParams.ReservedTo > c.ReservedFrom
                //         )
                //     )
                // )
                .OrderBy(o => o.OrderId)
                .ToListAsync();
        }

        public async Task<Order> GetOrder(int id)
        {
            var orderToReturn = await this.context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == id);
            return orderToReturn;
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