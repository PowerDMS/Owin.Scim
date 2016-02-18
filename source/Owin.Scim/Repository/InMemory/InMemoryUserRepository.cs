namespace Owin.Scim.Repository.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Model.Users;

    using NContext.Security.Cryptography;

    public class InMemoryUserRepository : IUserRepository
    {
        private readonly IDictionary<string, User> _Users;

        public InMemoryUserRepository()
        {
            _Users = new Dictionary<string, User>();
        }

        public async Task<User> CreateUser(User user)
        {
            user.Id = Guid.NewGuid().ToString();

            _Users.Add(user.Id, user);

            return user;
        }

        public async Task<User> GetUser(string userId)
        {
            return _Users.ContainsKey(userId) ? _Users[userId] : null;
        }

        public async Task UpdateUser(User user)
        {
            if (!_Users.ContainsKey(user.Id)) return;

            _Users[user.Id] = user;
        }

        public async Task<User> DeleteUser(string userId)
        {
            if (!_Users.ContainsKey(userId)) return null;

            var userRecord = _Users[userId];
            _Users.Remove(userId);

            return userRecord;
        }

        public async Task<bool> IsUserNameAvailable(string userName)
        {
            /* Before comparing or evaluating the uniqueness of a "userName" or 
               "password" attribute, service providers MUST use the preparation, 
               enforcement, and comparison of internationalized strings (PRECIS) 
               preparation and comparison rules described in Sections 3 and 4, 
               respectively, of [RFC7613], which is based on the PRECIS framework
               specification [RFC7564]. */

            return _Users
                .Values
                .All(u => !CryptographyUtility.CompareBytes(Encoding.UTF8.GetBytes(u.UserName), Encoding.UTF8.GetBytes(userName)));
        }
    }
}