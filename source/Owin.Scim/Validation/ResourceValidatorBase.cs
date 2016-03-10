namespace Owin.Scim.Validation
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentValidation;

    using Model;

    using NContext.Common;

    using Owin.Scim.Extensions;

    public abstract class ResourceValidatorBase<T> : ValidatorBase<T>, IValidator
        where T : Resource
    {
        private readonly ResourceExtensionValidators _ExtensionValidators;

        protected ResourceValidatorBase(ResourceExtensionValidators extensionValidators)
        {
            _ExtensionValidators = extensionValidators;
            CascadeMode = CascadeMode.StopOnFirstFailure;
            
            // Virtual member call from ctor but derived types should not require 
            // construction to configure rulesets. This MAY be short-sighted, however, 
            // dependencies should not need to be used during construction; but referenced from 
            // lambdas defined during construction yet invoked during validation.
            // -DGioulakis
            RuleSet("default", ConfigureDefaultRuleSet);
            RuleSet("create", ConfigureCreateRuleSet);
            RuleSet("update", ConfigureUpdateRuleSet);

            When(res => res.Extensions != null && res.Extensions.Any(),
                () =>
                {
                    RuleFor(res => res.Extensions)
                        .NestedRules(v =>
                            v.When(ext => ext.Value.IsValueCreated, () =>
                            {
                                v.RuleFor(ext => ext.Value.Value)
                                    .SetValidator2(ext => _ExtensionValidators[ext.Key]);
                            }));
                });
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

        public Type TargetType
        {
            get { return typeof (T); }
        }
    }
}