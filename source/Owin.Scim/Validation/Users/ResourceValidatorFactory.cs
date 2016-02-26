namespace Owin.Scim.Validation.Users
{
    using System.Threading.Tasks;

    using FluentValidation;

    using Model;
    using Model.Users;

    using Repository;

    using Security;

    public class ResourceValidatorFactory
    {
        private readonly IUserRepository _UserRepository;

        private readonly IVerifyPasswordComplexity _PasswordComplexityVerifier;

        private readonly IManagePasswords _PasswordManager;

        public ResourceValidatorFactory(
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
            if (resource is EnterpriseUser)
            {
                return Task.FromResult(
                    (IValidator)new EnterpriseUserValidator(
                        _UserRepository,
                        new UserValidator(
                            _UserRepository,
                            _PasswordComplexityVerifier,
                            _PasswordManager)));
            }

            return Task.FromResult(
                (IValidator)new UserValidator(
                    _UserRepository,
                    _PasswordComplexityVerifier,
                    _PasswordManager));
        }
    }
}