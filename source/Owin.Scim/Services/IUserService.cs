namespace Owin.Scim.Services
{
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Model;
    using Model.Users;

    public interface IUserService
    {
        Task<IScimResponse<User>> CreateUser(User user);

        Task<User> RetrieveUser(string userId);

        Task<IScimResponse<User>> UpdateUser(User user);

        Task<Unit> DeleteUser(string userId);
    }
}