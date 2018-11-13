using System;
using System.Collections.Generic;
using System.Linq;
using PRAPI.Models;
using PRAPI.Helpers;
using PRAPI.Data;

namespace PRAPI.Services
{
    public interface IUserService
    {
        User Login(string username, string password);
        // IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        void Update(User user, string password = null);
        // void Delete(int id);
    }
 
    public class UserService : IUserService
    {
        private DataContext context;
 
        public UserService(DataContext context)
        {
            this.context = context;
        }
 
        public User Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;
 
            var user = this.context.Users.SingleOrDefault(x => x.Username == username);
 
            // check if username exists
            if (user == null)
                return null;

            // only one user
            // if (user.Username != "moderator")
            //     return null;
 
            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
 
            // authentication successful
            return user;
        }
 
        // public IEnumerable<User> GetAll()
        // {
        //     return this.context.Users;
        // }
 
        public User GetById(int id)
        {
            return this.context.Users.Find(id);
        }
 
        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");
 
            if (this.context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");
 
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
 
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
 
            this.context.Users.Add(user);
            this.context.SaveChanges();
 
            return user;
        }
 
        public void Update(User userParam, string password = null)
        {
            var user = this.context.Users.Find(userParam.Id);
 
            if (user == null)
                throw new AppException("User not found");
 
            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (this.context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");
            }
 
            // update user properties
            user.Username = userParam.Username;
 
            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);
 
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
 
            this.context.Users.Update(user);
            this.context.SaveChanges();
        }
 
        // public void Delete(int id)
        // {
        //     var user = this.context.Users.Find(id);
        //     if (user != null)
        //     {
        //         this.context.Users.Remove(user);
        //         this.context.SaveChanges();
        //     }
        // }
 
        // private helper methods
 
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
 
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
 
        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");
 
            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
 
            return true;
        }
    }
}