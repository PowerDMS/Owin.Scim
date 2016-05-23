namespace Owin.Scim.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model.Users;

    using Querying;

    public interface IUserRepository
    {
        /// <summary>
        /// Persists the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<ScimUser> CreateUser(ScimUser user);

        /// <summary>
        /// Gets the <see cref="ScimUser"/> resource associated with the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ScimUser> GetUser(string userId);

        /// <summary>
        /// Updates the specified <paramref name="user"/> record.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task UpdateUser(ScimUser user);

        /// <summary>
        /// Clients request resource removal via DELETE.  Service providers MAY
        /// choose not to permanently delete the resource but MUST return a 404
        /// (Not Found) error code for all operations associated with the
        /// previously deleted resource.Service providers MUST omit the
        /// resource from future query results.In addition, the service
        /// provider SHOULD NOT consider the deleted resource in conflict
        /// calculation.  For example, if a User resource is deleted, a CREATE
        /// request for a User resource with the same userName as the previously
        /// deleted resource SHOULD NOT fail with a 409 error due to userName
        /// conflict.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<ScimUser> DeleteUser(string userId);

        /// <summary>
        /// Searches for users whose metadata satisfy the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<IEnumerable<ScimUser>> QueryUsers(ScimQueryOptions options);

        /// <summary>
        /// Returns whether the specified <paramref name="userName"/> is available or already in use.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> IsUserNameAvailable(string userName);
    }
}