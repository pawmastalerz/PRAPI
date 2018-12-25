using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRAPI.Dtos;
using PRAPI.Models;

namespace PRAPI.Data
{
    public interface IOrderRepository
    {
        decimal CalculatePrice(double dayDifference, double price);
        bool CreateOrder(Order order);
        Task<List<Order>> GetAllOrdersForUser(int id);
        Task<List<Order>> GetCurrentOrdersForUser(int id);
        Task<bool> SaveAll();
    }
}