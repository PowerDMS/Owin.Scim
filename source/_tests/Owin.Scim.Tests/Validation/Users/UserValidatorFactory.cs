namespace Owin.Scim.Tests.Validation.Users
{
    using System.Threading.Tasks;

    using Configuration;

    using FluentValidation;

    using Model;

    using Repository;

    using Scim.Security;
    using Scim.Validation;

    using v2.Validation;

    public class UserValidatorFactory : IResourceValidatorFactory
    {
        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly IUserRepository _UserRepository;

        private readonly IManagePasswords _PasswordManager;

        public UserValidatorFactory(
            ScimServerConfiguration serverConfiguration,
            IUserRepository userRepository,
            IManagePasswords passwordManager)
        {
            _ServerConfiguration = serverConfiguration;
            _UserRepository = userRepository;
            _PasswordManager = passwordManager;
        }

        public virtual Task<IValidator> CreateValidator<TResource>(TResource resource) 
            where TResource : Resource
        {
            var userValidator = new ScimUser2Validator(
                _ServerConfiguration,
                new ResourceExtensionValidators(new [] { new EnterpriseUser2ExtensionValidator() }), 
                _UserRepository,
                _PasswordManager);

            return Task.FromResult((IValidator)userValidator);
        }
    }
}