namespace Owin.Scim.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Linq;
    using System.Threading.Tasks;

    using Configuration;

    using ErrorHandling;

    using Extensions;

    using Microsoft.FSharp.Core;

    using Model;
    using Model.Users;

    using NContext.Extensions;

    using Querying;

    using Repository;

    using Security;

    using Validation;

    public class UserService : ServiceBase, IUserService
    {
        private readonly ICanonicalizationService _CanonicalizationService;

        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;

        private readonly IResourceValidatorFactory _ResourceValidatorFactory;

        public UserService(
            ScimServerConfiguration serverConfiguration,
            IResourceVersionProvider versionProvider,
            ICanonicalizationService canonicalizationService,
            IResourceValidatorFactory resourceValidatorFactory,
            IUserRepository userRepository,
            IManagePasswords passwordManager)
            : base(serverConfiguration, versionProvider)
        {
            _CanonicalizationService = canonicalizationService;
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
            _ResourceValidatorFactory = resourceValidatorFactory;
        }

        public async Task<IScimResponse<User>> CreateUser(User user)
        {
            _CanonicalizationService.Canonicalize(user, ServerConfiguration.GetScimTypeDefinition(user.GetType()));

            var validator = await _ResourceValidatorFactory.CreateValidator(user);
            var validationResult = (await validator.ValidateCreateAsync(user)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors.First());
            
            var createdDate = DateTime.UtcNow;
            user.Meta = new ResourceMetadata(ScimConstants.ResourceTypes.User)
            {
                Created = createdDate,
                LastModified = createdDate
            };

            var userRecord = await _UserRepository.CreateUser(user);

            SetResourceVersion(user);

            return new ScimDataResponse<User>(userRecord);
        }

        public async Task<IScimResponse<User>> RetrieveUser(string userId)
        {
            var userRecord = await _UserRepository.GetUser(userId);
            if (userRecord == null)
                return new ScimErrorResponse<User>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(userId)));

            SetResourceVersion(userRecord);

            return new ScimDataResponse<User>(userRecord);
        }

        public async Task<IScimResponse<User>> UpdateUser(User user)
        {
            var userRecord = await _UserRepository.GetUser(user.Id);
            if (userRecord == null)
            {
                return new ScimErrorResponse<User>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(user.Id)));
            }

            user.Groups = userRecord.Groups; // user.Groups is readOnly and used here only for resource versioning
            user.Meta = new ResourceMetadata(ScimConstants.ResourceTypes.User)
            {
                Created = userRecord.Meta.Created,
                LastModified = userRecord.Meta.LastModified
            };

            _CanonicalizationService.Canonicalize(user, ServerConfiguration.GetScimTypeDefinition(user.GetType()));

            var validator = await _ResourceValidatorFactory.CreateValidator(user);
            var validationResult = (await validator.ValidateUpdateAsync(user, userRecord)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors.First());
            
            // check if we're changing a password
            if (_PasswordManager.PasswordIsDifferent(user.Password, userRecord.Password))
            {
                if (!ServerConfiguration.GetFeature(ScimFeatureType.ChangePassword).Supported)
                {
                    return new ScimErrorResponse<User>(
                        new ScimError(
                            HttpStatusCode.BadRequest,
                            ScimErrorType.InvalidValue,
                            "Password change is not supported."));
                }

                // if we're not setting password to null, then hash the plainText
                if (user.Password != null)
                    user.Password = _PasswordManager.CreateHash(user.Password);
            }

            SetResourceVersion(user);

            // if both versions are equal, bypass persistence
            if (user.Meta.Version.Equals(userRecord.Meta.Version))
                return new ScimDataResponse<User>(user);

            user.Meta.LastModified = DateTime.UtcNow;

            await _UserRepository.UpdateUser(user);

            return new ScimDataResponse<User>(user);
        }

        public async Task<IScimResponse<Unit>> DeleteUser(string userId)
        {
            var result = await _UserRepository.DeleteUser(userId);
            if (result == null)
                return new ScimErrorResponse<Unit>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ErrorDetail.NotFound(userId)));

            return new ScimDataResponse<Unit>(default(Unit));
        }

        public async Task<IScimResponse<IEnumerable<User>>> QueryUsers(ScimQueryOptions options)
        {
            var users = await _UserRepository.QueryUsers(options) ?? new List<User>();
            users.ForEach(user => SetResourceVersion(user));

            return new ScimDataResponse<IEnumerable<User>>(users);
        }
    }
}