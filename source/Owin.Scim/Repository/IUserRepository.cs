namespace Owin.Scim.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model.Users;

    using Querying;

    public interface IUserRepository
    {
        Task<User> CreateUser(User user);

        Task<User> GetUser(string userId);

        Task UpdateUser(User user);

        Task<User> DeleteUser(string userId);

        Task<IEnumerable<User>> QueryUsers(ScimQueryOptions options);

        Task<bool> IsUserNameAvailable(string userName);
    }
}