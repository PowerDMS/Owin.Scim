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

        public async Task<IScimResponse<ScimUser>> CreateUser(ScimUser user)
        {
            _CanonicalizationService.Canonicalize(user, ServerConfiguration.GetScimTypeDefinition(user.GetType()));

            var validator = await _ResourceValidatorFactory.CreateValidator(user);
            var validationResult = (await validator.ValidateCreateAsync(user)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<ScimUser>(validationResult.Errors.First());

            if (user.Password != null)
                user.Password = _PasswordManager.CreateHash(user.Password);

            var createdDate = DateTime.UtcNow;
            user.Meta = new ResourceMetadata(ScimConstants.ResourceTypes.User)
            {
                Created = createdDate,
                LastModified = createdDate
            };

            var userRecord = await _UserRepository.CreateUser(user);
            if (userRecord == null)
                return new ScimErrorResponse<ScimUser>(
                    new ScimError(
                        HttpStatusCode.BadRequest));

            // version may require the User.Id which is often generated using database unique constraints
            // therefore, we will set the version after persistence
            SetResourceVersion(userRecord);

            return new ScimDataResponse<ScimUser>(userRecord);
        }

        public async Task<IScimResponse<ScimUser>> RetrieveUser(string userId)
        {
            var userRecord = await _UserRepository.GetUser(userId);
            if (userRecord == null)
                return new ScimErrorResponse<ScimUser>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ScimErrorDetail.NotFound(userId)));

            SetResourceVersion(userRecord);

            return new ScimDataResponse<ScimUser>(userRecord);
        }

        public async Task<IScimResponse<ScimUser>> UpdateUser(ScimUser user)
        {
            var userRecord = await _UserRepository.GetUser(user.Id);
            if (userRecord == null)
            {
                return new ScimErrorResponse<ScimUser>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ScimErrorDetail.NotFound(user.Id)));
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
                return new ScimErrorResponse<ScimUser>(validationResult.Errors.First());
            
            // check if we're changing a password
            if (_PasswordManager.PasswordIsDifferent(user.Password, userRecord.Password))
            {
                if (!ServerConfiguration.GetFeature(ScimFeatureType.ChangePassword).Supported)
                {
                    return new ScimErrorResponse<ScimUser>(
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
                return new ScimDataResponse<ScimUser>(user);

            user.Meta.LastModified = DateTime.UtcNow;

            await _UserRepository.UpdateUser(user);

            return new ScimDataResponse<ScimUser>(user);
        }

        public async Task<IScimResponse<Unit>> DeleteUser(string userId)
        {
            var result = await _UserRepository.DeleteUser(userId);
            if (result == null)
                return new ScimErrorResponse<Unit>(
                    new ScimError(
                        HttpStatusCode.NotFound,
                        detail: ScimErrorDetail.NotFound(userId)));

            return new ScimDataResponse<Unit>(default(Unit));
        }

        public async Task<IScimResponse<IEnumerable<ScimUser>>> QueryUsers(ScimQueryOptions options)
        {
            var users = await _UserRepository.QueryUsers(options) ?? new List<ScimUser>();
            users.ForEach(user => SetResourceVersion(user));

            return new ScimDataResponse<IEnumerable<ScimUser>>(users);
        }
    }
}