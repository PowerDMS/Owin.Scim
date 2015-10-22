namespace Owin.Scim.Repository
{
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Model.Users;

    public interface IUserRepository
    {
        Task<User> CreateUser(User user);

        Task<User> GetUser(string userId);

        Task UpdateUser(User user);

        Task<User> DeleteUser(string userId);

        Task<bool> IsUserNameAvailable(string userName);
    }
}