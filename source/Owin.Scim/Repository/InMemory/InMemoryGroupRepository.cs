using System.Linq;

namespace Owin.Scim.Repository.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Model.Groups;
    using Model.Users;

    /// <summary>
    /// This could have been implemented by InMemoryUserRepository
    /// </summary>
    public class InMemoryGroupRepository : IGroupRepository
    {
        private readonly IDictionary<string, Group> _groups;

        public InMemoryGroupRepository()
        {
            _groups = new Dictionary<string, Group>();
        }

        public Task<Group> CreateGroup(Group group)
        {
            group.Id = Guid.NewGuid().ToString("N");
            
            // TODO: (CY) who should be responsible for populating Meta (framework or implementer?)
            group.Meta = new Model.ResourceMetadata(ScimConstants.ResourceTypes.Group)
            {
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            _groups.Add(group.Id, group);

            return Task.FromResult(group);
        }

        public Task<Group> GetGroup(string groupId)
        {
            if (_groups.ContainsKey(groupId))
            {
                return Task.FromResult(_groups[groupId].Copy());
            }

            return Task.FromResult<Group>(null);
        }

        public Task<Group> UpdateGroup(Group group)
        {
            if (!_groups.ContainsKey(group.Id)) return Task.FromResult<Group>(null);

            _groups[group.Id] = group;

            return Task.FromResult(group);
        }

        public Task<Group> DeleteGroup(string groupId)
        {
            if (!_groups.ContainsKey(groupId)) return Task.FromResult<Group>(null);

            var groupRecord = _groups[groupId];
            _groups.Remove(groupId);

            return Task.FromResult(groupRecord);
        }

        public Task<IEnumerable<UserGroup>> GetGroupsUserBelongsTo(string userId)
        {
            // TODO: (CY) need to add indirect groups too
            return Task.FromResult(_groups
                .Values
                .Where(g => g.Members.Any(m => m.Value.Equals(userId)))
                .Select(group => new UserGroup
                {
                    Value = group.Id,
                    Ref = "../Groups/" + group.Id,
                    Display = group.DisplayName,
                    Type = "direct"
                }));
        }
    }
}