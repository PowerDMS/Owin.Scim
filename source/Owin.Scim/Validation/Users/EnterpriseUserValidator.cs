namespace Owin.Scim.Validation.Users
{
    using FluentValidation;

    using Model.Users;

    using Repository;

    public class EnterpriseUserExtensionValidator : AbstractValidator<EnterpriseUserExtension>
    {
        private readonly IUserRepository _UserRepository;

        public EnterpriseUserExtensionValidator(IUserRepository userRepository)
        {
            _UserRepository = userRepository;
        }
    }
}