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

        public async Task<User> GetUser(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> UpdateUser(User user)
        {
            var userRecord = await _UserRepository.GetUser(user.Id);
            if (userRecord == null) return null;

            var updatedUser = Mapper.Map(user, userRecord);
            var validationResult = await _UserValidator.ValidateAsync(updatedUser, RuleSetConstants.Update);
            if (validationResult)
            {
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    updatedUser.Password = _PasswordManager.CreateHash(
                        Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(user.Password.Trim())));
                }

                await _UserRepository.UpdateUser(updatedUser);

                updatedUser.Password = null; // The password is writeOnly and MUST NOT be returned.

                return updatedUser;
            }

            throw new Exception(string.Join(Environment.NewLine, validationResult.ErrorMessages)); // TODO: (DG) implement actual exception / error handling
        }

        public async Task<Unit> DeleteUser(string userId)
        {
            throw new System.NotImplementedException();
        }
    }
}