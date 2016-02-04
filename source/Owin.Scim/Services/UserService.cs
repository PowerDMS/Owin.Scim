namespace Owin.Scim.Services
{
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Configuration;

    using ErrorHandling;

    using Extensions;

    using FluentValidation;
    using FluentValidation.Internal;

    using Microsoft.FSharp.Core;

    using Model;
    using Model.Users;

    using PhoneNumbers;

    using Repository;

    using Security;

    using Validation;
    using Validation.Users;

    using PhoneNumber = Model.Users.PhoneNumber;

    public class UserService : IUserService
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;

        private readonly UserValidatorFactory _UserValidatorFactory;

        public UserService(
            ScimServerConfiguration serverConfiguration,
            IUserRepository userRepository,
            IManagePasswords passwordManager,
            UserValidatorFactory userValidatorFactory)
        {
            _ServerConfiguration = serverConfiguration;
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
            _UserValidatorFactory = userValidatorFactory;
        }

        public async Task<IScimResponse<User>> CreateUser(User user)
        {
            await CanonicalizeUser(user);

            var validator = await _UserValidatorFactory.CreateValidator(user);
            var validationResult = (await validator.ValidateAsync(user, ruleSet: RuleSetConstants.Create)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors);
            
            var userRecord = await _UserRepository.CreateUser(user);

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

            return new ScimDataResponse<User>(userRecord);
        }

        public async Task<IScimResponse<User>> UpdateUser(User user)
        {
            var userRecord = await _UserRepository.GetUser(user.Id);
            if (userRecord == null) return null;

            await CanonicalizeUser(user);

            var validator = await _UserValidatorFactory.CreateValidator(user);
            var validationResult = (await validator.ValidateAsync(user, ruleSet: RuleSetConstants.Update)).ToScimValidationResult();

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors);

            if (!string.IsNullOrWhiteSpace(userRecord.Password))
            {
                userRecord.Password = _PasswordManager.CreateHash(
                    Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(user.Password.Trim())));
            }

            await _UserRepository.UpdateUser(userRecord);

            return new ScimDataResponse<User>(userRecord);
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

        protected virtual Task CanonicalizeUser(User user)
        {
            if (!string.IsNullOrWhiteSpace(user.Locale))
            {
                user.Locale = user.Locale.Replace('_', '-'); // Supports backwards compatability
            }

            // TODO: (DG) Create generic canonicalization rule for ScimServerConfig + multiValuedAttribute.Type.

            // ADDRESSES
            user.Addresses.Canonicalize(
                (Address attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
                (Address attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            // CERTIFICATES
            user.X509Certificates.Canonicalize(
                (X509Certificate attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
                (X509Certificate attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            // EMAILS
            user.Emails.Canonicalize(
                (Email attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
                (Email attribute, ref object state) =>
                {
                    if (string.IsNullOrWhiteSpace(attribute.Value)) return;

                    var atIndex = attribute.Value.IndexOf('@') + 1;
                    if (atIndex == 0) return; // IndexOf returned -1

                    var cEmail = attribute.Value.Substring(0, atIndex) + attribute.Value.Substring(atIndex).ToLower();
                    attribute.Display = cEmail;
                },
                (Email attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            // ENTITLEMENTS
            user.Entitlements.Canonicalize(
                (Entitlement attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
                (Entitlement attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            // INSTANT MESSAGE ADDRESSES
            user.Ims.Canonicalize(
                (InstantMessagingAddress attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
                (InstantMessagingAddress attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            // PHONE NUMBERS
            user.PhoneNumbers.Canonicalize(
                (PhoneNumber attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
                (PhoneNumber attribute, ref object state) =>
                {
                    if (!string.IsNullOrWhiteSpace(attribute.Value))
                    {
                        var normalized = PhoneNumberUtil.Normalize(attribute.Value);
                        attribute.Display = string.IsNullOrWhiteSpace(normalized)
                            ? null
                            : normalized;
                    }
                }, 
                (PhoneNumber attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            // PHOTOS
            user.Photos.Canonicalize(
                (Photo attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute),
                (Photo attribute, ref object state) => Canonicalization.Lowercase(attribute, photo => photo.Value),
                (Photo attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));

            // ROLES
            user.Roles.Canonicalize(
                (Role attribute, ref object state) => Canonicalization.EnforceMutabilityRules(attribute), 
                (Role attribute, ref object state) => Canonicalization.EnforceSinglePrimaryAttribute(attribute, ref state));
            
            return Task.FromResult(0);
        }
    }
}