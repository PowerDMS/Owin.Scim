namespace Owin.Scim.Repository.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Model.Users;

    public class InMemoryUserRepository : IUserRepository
    {
        private readonly IList<User> _Users;

        public InMemoryUserRepository()
        {
            _Users = new List<User>();
        }

        public async Task<User> CreateUser(User user)
        {
            user.Id = Guid.NewGuid().ToString();

            _Users.Add(user);

            return user;
        }

        public async Task<User> GetUser(string userId)
        {
            return _Users.SingleOrDefault(u => u.Id.Equals(userId));
        }

        public async Task UpdateUser(User user)
        {
            var userRecord = _Users.SingleOrDefault(u => u.Id.Equals(user.Id));
            if (userRecord == null) throw new Exception();

            userRecord = user;
        }

        public async Task<Unit> DeleteUser(string userId)
        {
            var userRecord = _Users.SingleOrDefault(u => u.Id.Equals(userId));
            if (userRecord == null) throw new Exception();

            _Users.Remove(userRecord);

            return default(Unit);
        }

        public async Task<bool> IsUserNameAvailable(string userName)
        {
            return _Users.All(u => !u.UserName.Equals(userName));
        }
    }
}