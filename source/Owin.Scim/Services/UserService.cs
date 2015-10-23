namespace Owin.Scim.Services
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using Configuration;

    using ErrorHandling;

    using Microsoft.FSharp.Core;

    using Model;
    using Model.Users;

    using NContext.Extensions;

    using Repository;

    using Security;

    using Validation;
    using Validation.Users;

    public class UserService : IUserService
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;

        private readonly UserValidator _UserValidator;

        public UserService(
            ScimServerConfiguration serverConfiguration,
            IUserRepository userRepository,
            IManagePasswords passwordManager,
            UserValidator userValidator)
        {
            _ServerConfiguration = serverConfiguration;
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
            _UserValidator = userValidator;
        }

        public async Task<IScimResponse<User>> CreateUser(User user)
        {
            await CanonicalizeUser(user);

            var newUser = Mapper.Map(user, new User()); // Replace all new User metadata according to SCIM rules concerning mutability.
            var validationResult = await _UserValidator.ValidateAsync(newUser, RuleSetConstants.Create);

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors);
            
            var userRecord = await _UserRepository.CreateUser(user);

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

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

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

            return new ScimDataResponse<User>(userRecord);
        }

        public async Task<IScimResponse<User>> UpdateUser(User user)
        {
            var userRecord = await _UserRepository.GetUser(user.Id);
            if (userRecord == null) return null;

            await CanonicalizeUser(user);

            Mapper.Map(user, userRecord); // Replace all userRecord metadata according to SCIM rules concerning mutability.
            var validationResult = await _UserValidator.ValidateAsync(userRecord, RuleSetConstants.Update);

            if (!validationResult)
                return new ScimErrorResponse<User>(validationResult.Errors);

            if (!string.IsNullOrWhiteSpace(userRecord.Password))
            {
                userRecord.Password = _PasswordManager.CreateHash(
                    Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(user.Password.Trim())));
            }

            await _UserRepository.UpdateUser(userRecord);

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

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

            // don't forget to enforce only one primary value for each multi value attribute
            
            if (!string.IsNullOrWhiteSpace(user.Locale))
            {
                user.Locale = user.Locale.Replace('_', '-'); // Supports backwards compatability
            }

            if (user.Emails != null && user.Emails.Any())
            {
                user.Emails.ForEach(email =>
                {
                    if (string.IsNullOrWhiteSpace(email.Value)) return;

                    var atIndex = email.Value.IndexOf('@') + 1;
                    if (atIndex == 0) return; // IndexOf returned -1

                    var cEmail = email.Value.Substring(0, atIndex) + email.Value.Substring(atIndex).ToLower();
                    email.Value = cEmail;

                    if (!string.IsNullOrWhiteSpace(email.Display))
                    {
                        atIndex = email.Display.IndexOf('@') + 1;
                        if (atIndex == 0) return; // IndexOf returned -1

                        cEmail = email.Display.Substring(0, atIndex) + email.Display.Substring(atIndex).ToLower();
                        email.Value = cEmail;
                    }
                });

                // TODO: (DG) support email canonical types
            }

            if (user.Photos != null && user.Photos.Any())
            {
                user.Photos.ForEach(photo =>
                {
                    if (string.IsNullOrWhiteSpace(photo.Value)) return;

                    photo.Value = photo.Value.ToLower();
                });

                // TODO: (DG) support photo canonical types
            }



            return Task.FromResult(0);
        }
    }
}