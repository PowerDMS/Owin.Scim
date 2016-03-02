namespace Owin.Scim.Repository
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model.Groups;

    public interface IGroupRepository
    {
        Task<Group> CreateGroup(Group group);

        Task<Group> GetGroup(string groupId);

        Task<Group> UpdateGroup(Group group);

        Task<Group> DeleteGroup(string groupId);

        Task<IEnumerable<Model.Users.UserGroup>> GetGroupsUserBelongsTo(string userId);
    }
}