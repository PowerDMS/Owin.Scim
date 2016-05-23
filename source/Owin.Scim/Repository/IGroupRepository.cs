namespace Owin.Scim.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model.Groups;
    using Model.Users;

    using Querying;

    public interface IGroupRepository
    {
        Task<ScimGroup> CreateGroup(ScimGroup group);

        Task<ScimGroup> GetGroup(string groupId);

        Task<ScimGroup> UpdateGroup(ScimGroup group);

        Task<ScimGroup> DeleteGroup(string groupId);

        Task<IEnumerable<ScimGroup>> QueryGroups(ScimQueryOptions options);

        Task<IEnumerable<UserGroup>> GetGroupsUserBelongsTo(string userId);
    }
}