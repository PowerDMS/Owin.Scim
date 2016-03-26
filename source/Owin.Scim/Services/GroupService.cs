﻿namespace Owin.Scim.Services
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.FSharp.Core;

    using Canonicalization;
    using Configuration;
    using Extensions;
    using ErrorHandling;
    using Model;
    using Model.Groups;
    using Repository;
    using Validation;

    public class GroupService : ServiceBase, IGroupService
    {
        private readonly DefaultCanonicalizationService _CanonicalizationService;

        private readonly IResourceValidatorFactory _ResourceValidatorFactory;

        private readonly IGroupRepository _GroupRepository;

        public GroupService(
            ScimServerConfiguration scimServerConfiguration,
            IResourceValidatorFactory resourceValidatorFactory,
            DefaultCanonicalizationService canonicalizationService,
            IGroupRepository groupRepository) 
            : base(scimServerConfiguration)
        {
            _GroupRepository = groupRepository;
            _ResourceValidatorFactory = resourceValidatorFactory;
            _CanonicalizationService = canonicalizationService;
        }

        public async Task<IScimResponse<Group>> CreateGroup(Group group)
        {
            _CanonicalizationService.Canonicalize(group, ScimServerConfiguration.GetScimTypeDefinition(typeof(Group)));

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
                        detail: ErrorDetail.NotFound(groupId)));

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
                        detail: ErrorDetail.NotFound(@group.Id)));
            }

            @group.Meta = new ResourceMetadata(ScimConstants.ResourceTypes.Group)
            {
                Created = groupRecord.Meta.Created,
                LastModified = groupRecord.Meta.LastModified
            };

            _CanonicalizationService.Canonicalize(group, ScimServerConfiguration.GetScimTypeDefinition(typeof(Group)));
            
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
                        detail: ErrorDetail.NotFound(groupId)));

            return new ScimDataResponse<Unit>(default(Unit));
        }
    }
}