namespace Owin.Scim.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;
    
    using Model.Users;

    using Querying;

    public interface IUserService
    {
        Task<IScimResponse<ScimUser>> CreateUser(ScimUser user);

        Task<IScimResponse<ScimUser>> RetrieveUser(string userId);

        Task<IScimResponse<ScimUser>> UpdateUser(ScimUser user);

        Task<IScimResponse<Unit>> DeleteUser(string userId);

        Task<IScimResponse<IEnumerable<ScimUser>>> QueryUsers(ScimQueryOptions options);
    }
}