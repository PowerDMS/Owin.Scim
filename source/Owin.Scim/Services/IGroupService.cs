namespace Owin.Scim.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Model.Groups;

    using Querying;

    public interface IGroupService
    {
        Task<IScimResponse<Group>> CreateGroup(Group group);

        Task<IScimResponse<Group>> RetrieveGroup(string groupId);

        Task<IScimResponse<Group>> UpdateGroup(Group group);

        Task<IScimResponse<Unit>> DeleteGroup(string groupId);

        Task<IScimResponse<IEnumerable<Group>>> QueryGroups(ScimQueryOptions options);
    }
}