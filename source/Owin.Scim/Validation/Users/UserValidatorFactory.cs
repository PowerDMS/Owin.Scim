namespace Owin.Scim.Validation.Users
{
    using System.Threading.Tasks;

    using FluentValidation;

    using Model.Users;

    using Repository;

    using Security;

    public class UserValidatorFactory
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

        public virtual Task<IValidator<TUser>> CreateValidator<TUser>(TUser entity) 
            where TUser : User
        {
            if (entity is EnterpriseUser)
            {
                return Task.FromResult(
                    (IValidator<TUser>)new FluentEnterpriseUserValidator(
                        _UserRepository,
                        new FluentUserValidator(
                            _UserRepository,
                            _PasswordComplexityVerifier,
                            _PasswordManager)));
            }

            return Task.FromResult(
                (IValidator<TUser>)new FluentUserValidator(
                    _UserRepository,
                    _PasswordComplexityVerifier,
                    _PasswordManager));
        }
    }
}