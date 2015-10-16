namespace Owin.Scim.Repository
{
    using System.Threading.Tasks;

    using Model.Users;

    public interface IUserRepository
    {
        Task<User> GetUser(string userId);

        Task<bool> IsUserNameAvailable(string userName, string userIdToExclude = null);
    }
}