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
        Task<List<Order>> GetAllOrders(int id);
        Task<List<Order>> AdminGetAllOrders();
        Task<List<Order>> AdminGetAllCurrentOrders();
        Task<List<Order>> GetCurrentOrders(int id);
        Order GetOrderById(int orderId);
        bool MarkAsReturned(int orderId);
        Task<bool> SaveAll();
    }
}