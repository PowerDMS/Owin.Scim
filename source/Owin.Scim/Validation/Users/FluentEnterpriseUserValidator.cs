namespace Owin.Scim.Validation.Users
{
    using System.Linq;
    using System.Threading.Tasks;

    using FluentValidation;
    using FluentValidation.Results;

    using Model.Users;

    using Repository;

    public class FluentEnterpriseUserValidator : ValidatorBase<EnterpriseUser>, IValidator<User>
    {
        private readonly IUserRepository _UserRepository;

        private readonly FluentUserValidator _UserValidator;

        public FluentEnterpriseUserValidator(
            IUserRepository userRepository,
            FluentUserValidator userValidator)
        {
            _UserRepository = userRepository;
            _UserValidator = userValidator;
            ConfigureDefaultRuleSet();
            ConfigureCreateRuleSet();
            ConfigureUpdateRuleSet();
        }

        private void ConfigureDefaultRuleSet()
        {
        }

        private void ConfigureCreateRuleSet()
        {
        }

        private void ConfigureUpdateRuleSet()
        {
        }

        public override async Task<FluentValidation.Results.ValidationResult> ValidateAsync(ValidationContext<EnterpriseUser> context)
        {
            var coreUserValidationResults = await _UserValidator.ValidateAsync(context.InstanceToValidate);
            var enterpriseValidationResults = await base.ValidateAsync(context);

            return new FluentValidation.Results.ValidationResult(
                coreUserValidationResults
                    .Errors
                    .Concat(enterpriseValidationResults.Errors));
        }

        public ValidationResult Validate(User instance)
        {
            return _UserValidator.Validate(instance);
        }

        public Task<ValidationResult> ValidateAsync(User instance)
        {
            return _UserValidator.ValidateAsync(instance);
        }
    }
}