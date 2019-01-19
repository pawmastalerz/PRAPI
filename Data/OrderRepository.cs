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
                    return Math.Round((decimal)((price * (dayDifference + 1)) * 0.95), 2, MidpointRounding.AwayFromZero);
                default:
                    return Math.Round((decimal)((price * (dayDifference + 1)) * 0.92), 2, MidpointRounding.AwayFromZero);
            }
        }

        public bool CreateOrder(Order order)
        {
            var sql = from o in this.context.Orders
                      where (this.context.Orders.Any(q => (
                            (q.CarId == order.CarId) &&
                            (q.UserId == order.UserId) &&
                            (q.IsReturned == "nie") && (
                            ((q.ReservedFrom < order.ReservedFrom) &&
                            (q.ReservedTo >= order.ReservedFrom)) ||
                            ((q.ReservedFrom <= order.ReservedTo) &&
                            (q.ReservedTo > order.ReservedTo)) ||
                            ((q.ReservedFrom >= order.ReservedFrom) &&
                            (q.ReservedTo <= order.ReservedTo))
                            )
                        ))
                      )
                      select o;

            var result = sql.ToList();

            if (result.Count == 0)
            {
                order.IsReturned = "nie";
                var dayDifference = (order.ReservedTo - order.ReservedFrom).TotalDays;
                var orderedCar = this.context.Cars.Find(order.CarId);
                order.TotalPrice = this.CalculatePrice(dayDifference, orderedCar.Price);
                this.context.Orders.Add(order);
                this.context.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<List<Order>> GetAllOrdersForUser(int userId)
        {
            var orders = await this.context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.CarOrdered)
            .OrderByDescending(o => o.OrderId)
            .ToListAsync();

            return orders;
        }

        public async Task<List<Order>> GetCurrentOrdersForUser(int userId)
        {
            var orders = await this.context.Orders
            .Where(o => o.IsReturned == "nie" && o.UserId == userId)
            .Include(o => o.CarOrdered)
            .OrderByDescending(o => o.OrderId)
            .ToListAsync();

            return orders;
        }

        public bool MarkAsReturned(int orderId)
        {
            var orderToMark = this.context.Orders.FirstOrDefault(o => o.OrderId == orderId);
            orderToMark.IsReturned = "tak";
            this.context.SaveChanges();
            return true;
        }

        public Order GetOrderById(int orderId)
        {
            var orderToReturn = this.context.Orders
                .FirstOrDefault(o => o.OrderId == orderId);
            return orderToReturn;
        }

        public async Task<List<Order>> AdminGetAllOrders()
        {
            var orders = await this.context.Orders
            .Include(o => o.CarOrdered)
            .OrderByDescending(o => o.OrderId)
            .ToListAsync();

            return orders;
        }
    }
}