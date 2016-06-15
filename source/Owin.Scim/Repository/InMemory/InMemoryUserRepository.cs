namespace Owin.Scim.Repository.InMemory
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Extensions;

    using Model.Users;

    using NContext.Security.Cryptography;

    using Querying;

    public class InMemoryUserRepository : IUserRepository
    {
        private readonly IGroupRepository _GroupRepository;

        private readonly ConcurrentDictionary<string, ScimUser> _Users;

        public InMemoryUserRepository(IGroupRepository groupRepository)
        {
            _GroupRepository = groupRepository;
            _Users = new ConcurrentDictionary<string, ScimUser>();
        }

        public async Task<ScimUser> CreateUser(ScimUser user)
        {
            user.Id = Guid.NewGuid().ToString("N");

            _Users.TryAdd(user.Id, user);

            return user;
        }

        public async Task<ScimUser> GetUser(string userId)
        {
            // return a deep-clone of the user object
            // since this is in-memory, we don't want any HTTP PATCH or other code to actually modify the
            // simulated database record stored in the list, unless done through create,update,delete
            var user = !_Users.ContainsKey(userId) 
                ? null 
                : _Users[userId].Copy();

            if (user != null)
                user.Groups = await _GroupRepository.GetGroupsUserBelongsTo(userId);

            return user;
        }

        public async Task<ScimUser> UpdateUser(ScimUser user)
        {
            if (!_Users.ContainsKey(user.Id))
                return user;

            _Users[user.Id] = user;

            return user;
        }

        public async Task DeleteUser(string userId)
        {
            if (!_Users.ContainsKey(userId)) return;

            ScimUser userRecord;
            _Users.TryRemove(userId, out userRecord);
        }

        public async Task<IEnumerable<ScimUser>> QueryUsers(ScimQueryOptions options)
        {
            var users = _Users.Values.AsEnumerable();
            if (options.Filter != null)
                users = users.Where(options.Filter.ToPredicate<ScimUser>()).ToList();
            
            // TODO: (DG) sorting
            if (options.SortBy != null)
            {
            }

            if (options.StartIndex > 1)
                users = users.Skip(options.StartIndex);

            if (options.Count > 0)
                users = users.Take(options.Count);

            return users;
        }

        public Task<bool> IsUserNameAvailable(string userName)
        {
            /* Before comparing or evaluating the uniqueness of a "userName" or 
               "password" attribute, service providers MUST use the preparation, 
               enforcement, and comparison of internationalized strings (PRECIS) 
               preparation and comparison rules described in Sections 3 and 4, 
               respectively, of [RFC7613], which is based on the PRECIS framework
               specification [RFC7564]. */

            var userNameBytes = Encoding.UTF8.GetBytes(userName);
            
            return Task.FromResult(
                _Users
                    .Values
                    .All(u => !CryptographyUtility.CompareBytes(Encoding.UTF8.GetBytes(u.UserName), userNameBytes)));
        }

        public Task<bool> UserExists(string userId)
        {
            return Task.FromResult(_Users.ContainsKey(userId));
        }
    }
}