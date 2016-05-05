namespace Owin.Scim.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model.Groups;
    using Model.Users;

    using Querying;

    public interface IGroupRepository
    {
        Task<Group> CreateGroup(Group group);

        Task<Group> GetGroup(string groupId);

        Task<Group> UpdateGroup(Group group);

        Task<Group> DeleteGroup(string groupId);

        Task<IEnumerable<Group>> QueryGroups(ScimQueryOptions options);

        Task<IEnumerable<UserGroup>> GetGroupsUserBelongsTo(string userId);
    }
}