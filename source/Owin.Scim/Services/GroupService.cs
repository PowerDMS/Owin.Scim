namespace Owin.Scim.Services
{
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Configuration;
    using Model.Groups;
    using Repository;

    public class GroupService : ServiceBase, IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(
            ScimServerConfiguration scimServerConfiguration, 
            IGroupRepository groupRepository) 
            : base(scimServerConfiguration)
        {
            // TODO: validation and metadata population
            _groupRepository = groupRepository;
        }

        public async Task<IScimResponse<Group>> CreateGroup(Group group)
        {
            var groupRecord = await _groupRepository.CreateGroup(group);
            return new ScimDataResponse<Group>(SetResourceVersion(groupRecord));
        }

        public Task<IScimResponse<Group>> RetrieveGroup(string groupId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IScimResponse<Group>> UpdateGroup(Group @group)
        {
            throw new System.NotImplementedException();
        }

        public Task<IScimResponse<Unit>> DeleteGroup(string groupId)
        {
            throw new System.NotImplementedException();
        }
    }
}