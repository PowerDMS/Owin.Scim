namespace Owin.Scim.Validation
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Messaging;
    using System.Threading;
    using System.Threading.Tasks;

    using Configuration;

    using FluentValidation;

    using Extensions;
    using Model;

    public abstract class ResourceValidatorBase<T> : ValidatorBase<T>, IValidator
        where T : Resource
    {
        private const string _ResourceInstanceKey = @"ValidationInstance";

        private readonly ScimServerConfiguration _ServerConfiguration;

        private readonly ResourceExtensionValidators _ExtensionValidators;

        protected ResourceValidatorBase(
            ScimServerConfiguration serverConfiguration,
            ResourceExtensionValidators extensionValidators)
        {
            _ServerConfiguration = serverConfiguration;
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
                            v.When(ext => ext.Value != null, () =>
                            {
                                v.RuleFor(ext => ext.Value)
                                    .SetValidatorNonGeneric(ext => _ExtensionValidators[ext.Value.SchemaIdentifier]);
                            }));
                });
        }

        protected abstract void ConfigureDefaultRuleSet();

        protected abstract void ConfigureCreateRuleSet();

        protected abstract void ConfigureUpdateRuleSet();

        protected T ExistingRecord
        {
            get { return CallContext.LogicalGetData(_ResourceInstanceKey) as T; }
        }
        
        Task<FluentValidation.Results.ValidationResult> IValidator.ValidateAsync(ValidationContext context, CancellationToken cancellation)
        {
            var svc = context as ScimValidationContext<T>;
            if (svc != null)
                return ValidateAsync(svc, cancellation);

            var vc = context as ValidationContext<T>;
            if (vc != null)
                return ValidateAsync(vc, cancellation);

            return ValidateAsync(
                new ValidationContext<T>((T)context.InstanceToValidate, context.PropertyChain, context.Selector), cancellation);
        }

        public sealed override Task<FluentValidation.Results.ValidationResult> ValidateAsync(
            ValidationContext<T> context,
            CancellationToken token = new CancellationToken())
        {
            var svc = context as ScimValidationContext<T>;
            if (svc != null)
            {
                CallContext.LogicalSetData(_ResourceInstanceKey, svc.ExistingResourceRecord);
            }

            return base.ValidateAsync(context, token)
                .ContinueWith(task =>
                {
                    CallContext.FreeNamedDataSlot(_ResourceInstanceKey);

                    return task.Result;
                },  token);
        }

        public Type TargetType
        {
            get { return typeof (T); }
        }

        protected ScimServerConfiguration ServerConfiguration
        {
            get { return _ServerConfiguration; }
        }
    }
}