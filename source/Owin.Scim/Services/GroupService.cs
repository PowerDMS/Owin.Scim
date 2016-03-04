namespace Owin.Scim.Services
{
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Configuration;
    using Extensions;
    using ErrorHandling;
    using Model;
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
            // TODO: (CY) should I canonize $ref to absolute uri? how about type to pascal case?

            var validator = await _resourceValidatorFactory.CreateValidator(group);
            var validationResult = (await validator.ValidateAsync(group, ruleSet: RuleSetConstants.Create)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<Group>(validationResult.Errors.First());

            var groupRecord = await _groupRepository.CreateGroup(group);
            return new ScimDataResponse<Group>(SetResourceVersion(groupRecord));
        }

        public async Task<IScimResponse<Group>> RetrieveGroup(string groupId)
        {
            var userRecord = SetResourceVersion(await _groupRepository.GetGroup(groupId));
            if (userRecord == null)
                return new ScimErrorResponse<Group>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(groupId)));

            return new ScimDataResponse<Group>(userRecord);
        }

        public async Task<IScimResponse<Group>> UpdateGroup(Group @group)
        {
            var groupRecord = await _groupRepository.GetGroup(@group.Id);
            if (groupRecord == null)
            {
                return new ScimErrorResponse<Group>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(@group.Id)));
            }

            // TODO: (CY) should I canonize $ref to absolute uri? how about type to pascal case?
            var validator = await _resourceValidatorFactory.CreateValidator(group);
            var validationResult = (await validator.ValidateAsync(group, ruleSet: RuleSetConstants.Create)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<Group>(validationResult.Errors.First());

            SetResourceVersion(@group);

            // if both versions are equal, bypass persistence
            if (@group.Meta.Version.Equals(groupRecord.Meta.Version))
                return new ScimDataResponse<Group>(groupRecord);

            await _groupRepository.UpdateGroup(@group);

            return new ScimDataResponse<Group>(@group);
        }

        public async Task<IScimResponse<Unit>> DeleteGroup(string groupId)
        {
            var result = await _groupRepository.DeleteGroup(groupId);
            if (result == null)
                return new ScimErrorResponse<Unit>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(groupId)));

            return new ScimDataResponse<Unit>(default(Unit));
        }
    }
}