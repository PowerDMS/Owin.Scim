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
            // TODO: (DG) Canonicalize user

            var newUser = Mapper.Map(user, new User()); // Replace all new User metadata according to SCIM rules concerning mutability.
            var validationResult = await _UserValidator.ValidateAsync(newUser, RuleSetConstants.Create);
            if (!validationResult) throw new Exception(); // TODO: (DG) NO EXCEPTIONS! MUST HANDLE BULK. MUST RETURN PROPER HTTP STATUS CODES.
            
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

            // TODO: (DG) Canonicalize user

            Mapper.Map(user, userRecord); // Replace all userRecord metadata according to SCIM rules concerning mutability.
            var validationResult = await _UserValidator.ValidateAsync(userRecord, RuleSetConstants.Update);
            if (!validationResult) throw new Exception(); // TODO: (DG) exception handling

            if (!string.IsNullOrWhiteSpace(userRecord.Password))
            {
                userRecord.Password = _PasswordManager.CreateHash(
                    Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(user.Password.Trim())));
            }

            await _UserRepository.UpdateUser(userRecord);

            userRecord.Password = null; // The password is writeOnly and MUST NOT be returned.

            return userRecord;
        }

        public async Task<Unit> DeleteUser(string userId)
        {
            var result = await _UserRepository.DeleteUser(userId);
            if (result == null) return null;

            return result;
        }
    }
}