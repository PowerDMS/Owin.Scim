namespace Owin.Scim.Services
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Configuration;
    using Extensions;
    using Model.Groups;
    using Repository;
    using Validation;

    public class GroupService : ServiceBase, IGroupService
    {
        private readonly IResourceValidatorFactory _resourceValidatorFactory;

        private readonly IGroupRepository _groupRepository;

        public GroupService(
            ScimServerConfiguration scimServerConfiguration,
            IResourceValidatorFactory resourceValidatorFactory,
            IGroupRepository groupRepository) 
            : base(scimServerConfiguration)
        {
            // TODO: validation and metadata population
            _groupRepository = groupRepository;
            _resourceValidatorFactory = resourceValidatorFactory;
        }

        public async Task<IScimResponse<Group>> CreateGroup(Group group)
        {
            var validator = await _resourceValidatorFactory.CreateValidator(group);
            var validationResult = (await validator.ValidateAsync(group, ruleSet: RuleSetConstants.Create)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<Group>(validationResult.Errors.First());

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