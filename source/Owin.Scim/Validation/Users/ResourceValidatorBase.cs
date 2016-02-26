namespace Owin.Scim.Validation.Users
{
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;
    using FluentValidation.Results;

    using Model;

    using NContext.Common;

    public abstract class ResourceValidatorBase<T> : ValidatorBase<T> where T : Resource
    {
        private bool _IsConfigured;


        public ResourceValidatorBase()
        {
        }

        protected string ResourceId { get; set; }

        protected abstract void ConfigureDefaultRuleSet();

        protected abstract void ConfigureCreateRuleSet();

        protected abstract void ConfigureUpdateRuleSet(AsyncLazy<T> resourceRecord);

        protected abstract Task<T> GetExistingResourceRecord();

        protected virtual Task<ValidationResult> PreValidate(
            ValidationContext<T> context, 
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(new ValidationResult());
        }

        public sealed override async Task<FluentValidation.Results.ValidationResult> ValidateAsync(
            ValidationContext<T> context,
            CancellationToken token = new CancellationToken())
        {
            ResourceId = context.InstanceToValidate.Id;

            var preValidationResult = await PreValidate(context, token);

            if (CascadeMode == CascadeMode.StopOnFirstFailure && !preValidationResult.IsValid)
                return preValidationResult;

            var validationResult = await base.ValidateAsync(context, token);
            validationResult.Errors.AddRangeC(preValidationResult.Errors);

            return validationResult;
        }
    }
}