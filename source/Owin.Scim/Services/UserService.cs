namespace Owin.Scim.Services
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;
    
    using Microsoft.FSharp.Core;

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

        public async Task<User> CreateUser(User user)
        {
            var newUser = Mapper.Map(user, new User()); // Replace all userRecord metadata according to SCIM rules concerning mutability.

            // TODO: (DG) Canonicalize user

            var userRecord = await _UserRepository.CreateUser(user);

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

            return userRecord;
        }

        public async Task<User> RetrieveUser(string userId)
        {
            var userRecord = await _UserRepository.GetUser(userId);
            if (userRecord == null) return null;

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

            return userRecord;
        }

        public async Task<User> UpdateUser(User user)
        {
            var userRecord = await _UserRepository.GetUser(user.Id);
            if (userRecord == null) return null;

            Mapper.Map(user, userRecord); // Replace all userRecord metadata according to SCIM rules concerning mutability.
            var validationResult = await _UserValidator.ValidateAsync(userRecord, RuleSetConstants.Update);
            if (validationResult)
            {
                if (!string.IsNullOrWhiteSpace(userRecord.Password))
                {
                    userRecord.Password = _PasswordManager.CreateHash(
                        Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(user.Password.Trim())));
                }

                // TODO: (DG) Canonicalize user

                await _UserRepository.UpdateUser(userRecord);

                userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

                return userRecord;
            }

            throw new Exception(string.Join(Environment.NewLine, validationResult.ErrorMessages)); // TODO: (DG) implement actual exception / error handling
        }

        public async Task<Unit> DeleteUser(string userId)
        {
            var result = await _UserRepository.DeleteUser(userId);
            if (result == null) return null;

            return result;
        }
    }
}