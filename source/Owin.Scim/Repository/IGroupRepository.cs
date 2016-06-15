namespace Owin.Scim.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model.Groups;
    using Model.Users;

    using Querying;

    public interface IGroupRepository
    {
        /// <summary>
        /// Persists the specified <paramref name="group"/>.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>Task&lt;ScimGroup&gt;.</returns>
        Task<ScimGroup> CreateGroup(ScimGroup group);

        /// <summary>
        /// Gets the <see cref="ScimGroup"/> resource associated with the specified <paramref name="groupId"/>.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>Task&lt;ScimGroup&gt;.</returns>
        Task<ScimGroup> GetGroup(string groupId);

        /// <summary>
        /// Updates the specified <paramref name="group"/> record.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>Task&lt;ScimGroup&gt;.</returns>
        Task<ScimGroup> UpdateGroup(ScimGroup group);

        /// <summary>
        /// Deletes the <see cref="ScimGroup"/> resource associated with the specified <paramref name="groupId"/>. 
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
        /// <param name="groupId">The group identifier.</param>
        /// <returns>Task.</returns>
        Task DeleteGroup(string groupId);

        /// <summary>
        /// Searches for groups whose metadata satisfy the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>Task&lt;IEnumerable&lt;ScimGroup&gt;&gt;.</returns>
        Task<IEnumerable<ScimGroup>> QueryGroups(ScimQueryOptions options);

        /// <summary>
        /// Gets the groups the specified <paramref name="userId"/> belongs to.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task&lt;IEnumerable&lt;UserGroup&gt;&gt;.</returns>
        Task<IEnumerable<UserGroup>> GetGroupsUserBelongsTo(string userId);

        /// <summary>
        /// Determines whether a group with the specified <paramref name="groupId"/> exists.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> GroupExists(string groupId);
    }
}