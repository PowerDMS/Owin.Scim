namespace Owin.Scim.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;
    
    using Model.Users;

    using Querying;

    public interface IUserService
    {
        Task<IScimResponse<User>> CreateUser(User user);

        Task<IScimResponse<User>> RetrieveUser(string userId);

        Task<IScimResponse<User>> UpdateUser(User user);

        Task<IScimResponse<Unit>> DeleteUser(string userId);

        Task<IScimResponse<IEnumerable<User>>> QueryUsers(ScimQueryOptions options);
    }
}