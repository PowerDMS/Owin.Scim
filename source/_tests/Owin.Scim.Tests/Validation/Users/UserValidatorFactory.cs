namespace Owin.Scim.Tests.Validation.Users
{
    using System.Threading.Tasks;

    using FluentValidation;

    using Model;

    using Repository;

    using Scim.Security;
    using Scim.Validation;
    using Scim.Validation.Users;

    public class UserValidatorFactory : IResourceValidatorFactory
    {
        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;

        public UserValidatorFactory(
            IUserRepository userRepository,
            IManagePasswords passwordManager)
        {
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
        }

        public virtual Task<IValidator> CreateValidator<TResource>(TResource resource) 
            where TResource : Resource
        {
            var userValidator = new UserValidator(
                new ResourceExtensionValidators(new [] { new EnterpriseUserExtensionValidator() }), 
                _UserRepository,
                _PasswordManager);

            return Task.FromResult((IValidator)userValidator);
        }
    }
}