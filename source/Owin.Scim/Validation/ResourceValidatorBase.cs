namespace Owin.Scim.Validation
{
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;

    using Model;

    using NContext.Common;

    public abstract class ResourceValidatorBase<T> : ValidatorBase<T>, IValidator 
        where T : Resource
    {
        private bool _IsConfigured;

        private readonly object _SyncLock = new object();
        
        protected ResourceValidatorBase()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
        }

        protected abstract void ConfigureDefaultRuleSet();

        protected abstract void ConfigureCreateRuleSet();

        protected abstract void ConfigureUpdateRuleSet();

        protected T ExistingRecord
        {
            get { return CallContext.LogicalGetData("resource") as T; }
        }

        protected virtual Task<FluentValidation.Results.ValidationResult> PreValidate(
            ValidationContext<T> context, 
            CancellationToken token = new CancellationToken())
        {
            return Task.FromResult(new FluentValidation.Results.ValidationResult());
        }
        
        Task<FluentValidation.Results.ValidationResult> IValidator.ValidateAsync(ValidationContext context, CancellationToken cancellation)
        {
            var svc = context as ScimValidationContext<T>;
            if (svc != null)
                return ValidateAsync(svc, cancellation);

            var vc = context as ValidationContext<T>;
            if (vc != null)
                return ValidateAsync(vc, cancellation);

            return ValidateAsync(new ValidationContext<T>((T)context.InstanceToValidate, context.PropertyChain, context.Selector), cancellation);
        }

        public sealed override async Task<FluentValidation.Results.ValidationResult> ValidateAsync(
            ValidationContext<T> context,
            CancellationToken token = new CancellationToken())
        {
            Configure();

            var svc = context as ScimValidationContext<T>;
            if (svc != null)
            {
                CallContext.LogicalSetData("resource", svc.ExistingResourceRecord);
            }
            
            var preValidationResult = await PreValidate(context, token);

            if (CascadeMode == CascadeMode.StopOnFirstFailure && !preValidationResult.IsValid)
                return preValidationResult;

            var validationResult = await base.ValidateAsync(context, token);
            validationResult.Errors.AddRangeC(preValidationResult.Errors);

            return validationResult;
        }

        internal void Configure()
        {
            if (!_IsConfigured)
            {
                lock (_SyncLock)
                {
                    if (!_IsConfigured)
                    {
                        ConfigureDefaultRuleSet();
                        ConfigureCreateRuleSet();
                        ConfigureUpdateRuleSet();

                        _IsConfigured = true;
                    }
                }
            }
        }
    }
}