namespace Owin.Scim.Services
{
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using ErrorHandling;

    using Microsoft.FSharp.Core;

    using Model;
    using Model.Users;

    using Repository;

    using Security;

    using Validation;
    using Validation.Users;

    public class UserService : IUserService
    {
        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;

        private readonly UserValidator _UserValidator;

        public UserService(
            IUserRepository userRepository,
            IManagePasswords passwordManager,
            UserValidator userValidator)
        {
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
            _UserValidator = userValidator;
        }

        public async Task<IScimResponse<User>> CreateUser(User user)
        {
            // TODO: (DG) Canonicalize user
            // don't forget to replace user.Locale _ underscore with hyphen!
            // don't forget to enforce only one primary value for each multi value attribute

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

            // TODO: (DG) Canonicalize user
            // don't forget to replace user.Locale _ underscore with hyphen!
            // don't forget to enforce only one primary value for each multi value attribute

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
    }
}