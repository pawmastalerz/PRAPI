using System;
using System.Collections.Generic;
using PRAPI.Models;

namespace PRAPI.Services
{
    public interface IUserService
    {
        User Login(string username, string password);
        User Create(User user, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        void Update(User user, string password = null);
        void Delete(int id);
    }
}