using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRAPI.Models;

namespace PRAPI.Services
{
    public interface IUserService
    {
        User Login(string username, string password);
        User Create(User user, string password);
        User GetById(int userId);
        void Update(User user, string currentPassword = null, string password = null);
        bool PurgeDatabase(int userId);
        void Delete(int userId);
        List<User> AdminGetAllUsers();
    }
}