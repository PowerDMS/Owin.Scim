namespace Owin.Scim.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;
    
    using Configuration;
    using Extensions;
    using ErrorHandling;
    using Model;
    using Model.Groups;

    using NContext.Extensions;

    using Querying;

    using Repository;
    using Validation;

    public class GroupService : ServiceBase, IGroupService
    {
        private readonly ICanonicalizationService _CanonicalizationService;

        private readonly IResourceValidatorFactory _ResourceValidatorFactory;

        private readonly IGroupRepository _GroupRepository;

        public GroupService(
            ScimServerConfiguration serverConfiguration,
            IResourceVersionProvider versionProvider,
            IResourceValidatorFactory resourceValidatorFactory,
            ICanonicalizationService canonicalizationService,
            IGroupRepository groupRepository) 
            : base(serverConfiguration, versionProvider)
        {
            _GroupRepository = groupRepository;
            _ResourceValidatorFactory = resourceValidatorFactory;
            _CanonicalizationService = canonicalizationService;
        }

        public async Task<IScimResponse<Group>> CreateGroup(Group group)
        {
            _CanonicalizationService.Canonicalize(group, ServerConfiguration.GetScimTypeDefinition(group.GetType()));

            var validator = await _ResourceValidatorFactory.CreateValidator(group);
            var validationResult = (await validator.ValidateCreateAsync(group)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<Group>(validationResult.Errors.First());

            var createdDate = DateTime.UtcNow;
            group.Meta = new ResourceMetadata(ScimConstants.ResourceTypes.Group)
            {
                Created = createdDate,
                LastModified = createdDate
            };
            
            var groupRecord = await _GroupRepository.CreateGroup(group);

            SetResourceVersion(groupRecord);

            return new ScimDataResponse<Group>(groupRecord);
        }

        public async Task<IScimResponse<Group>> RetrieveGroup(string groupId)
        {
            var userRecord = SetResourceVersion(await _GroupRepository.GetGroup(groupId));
            if (userRecord == null)
                return new ScimErrorResponse<Group>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ScimErrorDetail.NotFound(groupId)));

            return new ScimDataResponse<Group>(userRecord);
        }

        public async Task<IScimResponse<Group>> UpdateGroup(Group @group)
        {
            var groupRecord = await _GroupRepository.GetGroup(@group.Id);
            if (groupRecord == null)
            {
                return new ScimErrorResponse<Group>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ScimErrorDetail.NotFound(@group.Id)));
            }

            @group.Meta = new ResourceMetadata(ScimConstants.ResourceTypes.Group)
            {
                Created = groupRecord.Meta.Created,
                LastModified = groupRecord.Meta.LastModified
            };

            _CanonicalizationService.Canonicalize(group, ServerConfiguration.GetScimTypeDefinition(typeof(Group)));
            
            var validator = await _ResourceValidatorFactory.CreateValidator(group);
            var validationResult = (await validator.ValidateUpdateAsync(group, groupRecord)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<Group>(validationResult.Errors.First());
            
            SetResourceVersion(@group);

            // if both versions are equal, bypass persistence
            if (@group.Meta.Version.Equals(groupRecord.Meta.Version))
                return new ScimDataResponse<Group>(groupRecord);

            @group.Meta.LastModified = DateTime.UtcNow;

            await _GroupRepository.UpdateGroup(@group);

            return new ScimDataResponse<Group>(@group);
        }

        public async Task<IScimResponse<Unit>> DeleteGroup(string groupId)
        {
            var result = await _GroupRepository.DeleteGroup(groupId);
            if (result == null)
                return new ScimErrorResponse<Unit>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ScimErrorDetail.NotFound(groupId)));

            return new ScimDataResponse<Unit>(default(Unit));
        }

        public async Task<IScimResponse<IEnumerable<Group>>> QueryGroups(ScimQueryOptions options)
        {
            var groups = await _GroupRepository.QueryGroups(options) ?? new List<Group>();
            groups.ForEach(group => SetResourceVersion(group));

            return new ScimDataResponse<IEnumerable<Group>>(groups);
        }
    }
}