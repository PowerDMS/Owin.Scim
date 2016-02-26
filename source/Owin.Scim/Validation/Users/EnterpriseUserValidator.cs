namespace Owin.Scim.Validation.Users
{
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;
    using FluentValidation.Results;

    using Model.Users;

    using Repository;

    public class EnterpriseUserValidator : ResourceValidatorBase<EnterpriseUser>
    {
        private readonly IUserRepository _UserRepository;

        private readonly UserValidator _UserValidator;

        public EnterpriseUserValidator(
            IUserRepository userRepository,
            UserValidator userValidator)
        {
            _UserRepository = userRepository;
            _UserValidator = userValidator;
        }
        
        protected override void ConfigureDefaultRuleSet()
        {
        }

        protected override void ConfigureCreateRuleSet()
        {
        }

        protected override void ConfigureUpdateRuleSet()
        {
        }

        protected override Task<ValidationResult> PreValidate(
            ValidationContext<EnterpriseUser> context,
            CancellationToken token = new CancellationToken())
        {
            return _UserValidator.ValidateAsync(
                new ValidationContext<User>(context.InstanceToValidate, null, context.Selector),
                token);
        }
    }
}