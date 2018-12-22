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
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext context;
        private readonly IHostingEnvironment hostingEnvironment;

        public OrderRepository(DataContext context, IHostingEnvironment hostingEnvironment)
        {
            this.context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<bool> SaveAll()
        {
            var saved = await this.context.SaveChangesAsync() > 0;
            return saved;
        }

        public decimal CalculatePrice(double dayDifference, double price)
        {
            switch (dayDifference)
            {
                case 0:
                    return Math.Round((decimal)(price), 2, MidpointRounding.AwayFromZero);
                case 1:
                case 2:
                    return Math.Round((decimal)((price * dayDifference) * 0.95), 2, MidpointRounding.AwayFromZero);
                default:
                    return Math.Round((decimal)((price * dayDifference) * 0.92), 2, MidpointRounding.AwayFromZero);
            }
        }

        public bool CreateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetAllOrdersForUser(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetCurrentOrdersForUser(int id)
        {
            throw new NotImplementedException();
        }
    }
}