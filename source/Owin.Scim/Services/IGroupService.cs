namespace Owin.Scim.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Model.Groups;

    using Querying;

    public interface IGroupService
    {
        Task<IScimResponse<ScimGroup>> CreateGroup(ScimGroup group);

        Task<IScimResponse<ScimGroup>> RetrieveGroup(string groupId);

        Task<IScimResponse<ScimGroup>> UpdateGroup(ScimGroup group);

        Task<IScimResponse<Unit>> DeleteGroup(string groupId);

        Task<IScimResponse<IEnumerable<ScimGroup>>> QueryGroups(ScimQueryOptions options);
    }
}