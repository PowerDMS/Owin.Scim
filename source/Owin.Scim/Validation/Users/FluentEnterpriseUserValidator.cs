namespace Owin.Scim.Validation.Users
{
    using System.Linq;
    using System.Threading.Tasks;

    using FluentValidation;

    using Model.Users;

    public class FluentEnterpriseUserValidator : ValidatorBase<EnterpriseUser>
    {
        private readonly FluentUserValidator _UserValidator;

        public FluentEnterpriseUserValidator(FluentUserValidator userValidator)
        {
            _UserValidator = userValidator;
        }
            
        public override async Task<FluentValidation.Results.ValidationResult> ValidateAsync(ValidationContext<EnterpriseUser> context)
        {
            var results = await _UserValidator.ValidateAsync(context.InstanceToValidate);

            var eResults = await base.ValidateAsync(context);

            return new FluentValidation.Results.ValidationResult(
                results.Errors.Concat(eResults.Errors));
        }
    }
}