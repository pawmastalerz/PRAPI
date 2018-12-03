using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRAPI.Models;

namespace PRAPI.Data
{
    public interface IOrderRepository
    {
        bool CreateOrder(Order order);
        Task<Order> GetOrder(int id);
        Task<List<Order>> GetAllOrders();
        Task<List<Order>> SearchForUserOrders(SearchParams searchParams);
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
    }
}