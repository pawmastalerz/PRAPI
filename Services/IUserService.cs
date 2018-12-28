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
        User GetById(int userId);
        void Update(User user, string currentPassword = null, string password = null);
        bool PurgeDatabase(int userId);
        void Delete(int userId);
    }
}