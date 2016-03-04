namespace Owin.Scim.Validation
{
    using System.Runtime.Remoting.Messaging;

    using FluentValidation;

    using Model;

    public abstract class ResourceExtensionValidatorBase<TResource, TExtension> 
        : AbstractValidator<TExtension>, IResourceExtensionValidator
        where TResource : Resource
        where TExtension : ResourceExtension
    {
        protected ResourceExtensionValidatorBase()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            // Virtual member call from ctor but derived types should not require 
            // construction to configure rulesets. This MAY be short-sighted, however, 
            // dependencies should not need to be used during construction; but referenced from 
            // lambdas defined during construction yet invoked during validation.
            // -DGioulakis
            RuleSet("default", ConfigureDefaultRuleSet);
            RuleSet("create", ConfigureCreateRuleSet);
            RuleSet("update", ConfigureUpdateRuleSet);
        }

        public abstract string ExtensionSchema { get; }

        protected abstract void ConfigureDefaultRuleSet();

        protected abstract void ConfigureCreateRuleSet();

        protected abstract void ConfigureUpdateRuleSet();

        protected TResource ExistingRecord
        {
            get { return CallContext.LogicalGetData("resource") as TResource; }
        }
    }
}