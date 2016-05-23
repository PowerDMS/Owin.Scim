namespace Owin.Scim.Repository.InMemory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Extensions;

    using Model.Groups;
    using Model.Users;

    using Querying;

    /// <summary>
    /// This could have been implemented by InMemoryUserRepository
    /// </summary>
    public class InMemoryGroupRepository : IGroupRepository
    {
        private readonly IDictionary<string, ScimGroup> _Groups;

        public InMemoryGroupRepository()
        {
            _Groups = new Dictionary<string, ScimGroup>();
        }

        public async Task<ScimGroup> CreateGroup(ScimGroup group)
        {
            group.Id = Guid.NewGuid().ToString("N");

            _Groups.Add(group.Id, group);

            return group;
        }

        public async Task<ScimGroup> GetGroup(string groupId)
        {
            if (_Groups.ContainsKey(groupId))
                return _Groups[groupId].Copy();

            return null;
        }

        public async Task<ScimGroup> UpdateGroup(ScimGroup group)
        {
            if (!_Groups.ContainsKey(group.Id))
                return null;

            _Groups[group.Id] = group;

            return group;
        }

        public async Task<ScimGroup> DeleteGroup(string groupId)
        {
            if (!_Groups.ContainsKey(groupId))
                return null;

            var groupRecord = _Groups[groupId];
            _Groups.Remove(groupId);

            return groupRecord;
        }

        public async Task<IEnumerable<ScimGroup>> QueryGroups(ScimQueryOptions options)
        {
            var groups = _Groups.Values.AsEnumerable();
            if (options.Filter != null)
                groups = groups.Where(options.Filter.ToPredicate<ScimGroup>()).ToList();

            // TODO: (DG) sorting
            if (options.SortBy != null)
            {
            }

            if (options.StartIndex > 1)
                groups = groups.Skip(options.StartIndex);

            if (options.Count > 0)
                groups = groups.Take(options.Count);

            return groups;
        }

        public async Task<IEnumerable<UserGroup>> GetGroupsUserBelongsTo(string userId)
        {
            // TODO: (CY) need to add indirect groups too
            return _Groups
                .Values
                .Where(g => g.Members != null && g.Members.Any(m => m.Value.Equals(userId)))
                .Select(group => new UserGroup
                {
                    Value = group.Id,
                    Ref = new Uri("../Groups/" + group.Id),
                    Display = group.DisplayName,
                    Type = "direct"
                });
        }
    }
}