namespace Owin.Scim.Tests.Validation.Users
{
    using System.Threading.Tasks;

    using FluentValidation;

    using Model;

    using Repository;

    using Scim.Validation;
    using Scim.Validation.Users;

    using Security;

    public class UserValidatorFactory : IResourceValidatorFactory
    {
        private readonly IUserRepository _UserRepository;

        private readonly IVerifyPasswordComplexity _PasswordComplexityVerifier;

        private readonly IManagePasswords _PasswordManager;

        public UserValidatorFactory(
            IUserRepository userRepository,
            IVerifyPasswordComplexity passwordComplexityVerifier,
            IManagePasswords passwordManager)
        {
            _UserRepository = userRepository;
            _PasswordComplexityVerifier = passwordComplexityVerifier;
            _PasswordManager = passwordManager;
        }

        public virtual Task<IValidator> CreateValidator<TResource>(TResource resource) 
            where TResource : Resource
        {
            var userValidator = new UserValidator(
                _UserRepository,
                _PasswordComplexityVerifier,
                _PasswordManager);

            return Task.FromResult((IValidator)userValidator);
        }
    }
}