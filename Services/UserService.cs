using System;
using System.Collections.Generic;
using System.Linq;
using PRAPI.Models;
using PRAPI.Helpers;
using PRAPI.Data;
using System.Security.Authentication;
using Hangfire;

namespace PRAPI.Services
{
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

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return this.context.Users;
        }

        public User GetById(int userId)
        {
            return this.context.Users.Find(userId);
        }

        public User Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (this.context.Users.Any(x => x.Username == user.Username))
                throw new AppException("This username is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            this.context.Users.Add(user);
            this.context.SaveChanges();

            RecurringJob.AddOrUpdate(() =>
                    this.PurgeDatabase(user.UserId),
                    "00 23 * * *", TimeZoneInfo.Local);

            return user;
        }

        public void Update(User userParam, string currentPassword = null, string password = null)
        {
            var user = this.context.Users.Find(userParam.UserId);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                if (this.context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");
            }

            if (!string.IsNullOrWhiteSpace(userParam.Username))
                user.Username = userParam.Username;
            if (!string.IsNullOrWhiteSpace(userParam.City))
                user.City = userParam.City;
            if (!string.IsNullOrWhiteSpace(userParam.Street))
                user.Street = userParam.Street;
            if (!string.IsNullOrWhiteSpace(userParam.StreetNumber))
                user.StreetNumber = userParam.StreetNumber;
            if (!string.IsNullOrWhiteSpace(userParam.PostalCode))
                user.PostalCode = userParam.PostalCode;

            if (!string.IsNullOrWhiteSpace(password))
            {
                if (!VerifyPasswordHash(currentPassword, user.PasswordHash, user.PasswordSalt))
                    throw new InvalidCredentialException();

                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            this.context.Users.Update(user);
            this.context.SaveChanges();
        }

        public bool PurgeDatabase(int userId)
        {
            try
            {
                var ordersToDelete = this.context.Orders
                .Where(o => o.UserId == userId)
                .ToList();

                var userToDelete = this.context.Users
                .FirstOrDefault(u => u.UserId == userId);

                if (ordersToDelete != null)
                {
                    this.context.Orders.RemoveRange(ordersToDelete);
                    this.context.Users.Remove(userToDelete);
                    this.context.SaveChanges();
                }

                Console.WriteLine("Usunięto zamówienia i dane użytkownika o id " + userId + ".");
                return true;
            }
            catch (System.Exception)
            {
                RecurringJob.AddOrUpdate(() =>
                    this.PurgeDatabase(userId),
                    "00 23 * * *", TimeZoneInfo.Local);

                return false;
            }
        }

        public void Delete(int userId)
        {
            var user = this.context.Users.Find(userId);
            if (user != null)
            {
                this.context.Users.Remove(user);
                this.context.SaveChanges();
            }
        }

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
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64)
                throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128)
                throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

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