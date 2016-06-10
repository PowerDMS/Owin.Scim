namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System.Net;

    using Configuration;

    using ErrorHandling;

    using FluentValidation;

    using Model;

    using Scim.Validation;

    public class TenantValidator : ResourceValidatorBase<Tenant>
    {
        public TenantValidator(
            ScimServerConfiguration serverConfiguration,
            ResourceExtensionValidators extensionValidators)
            : base(serverConfiguration, extensionValidators)
        {
        }

        protected override void ConfigureDefaultRuleSet()
        {
            RuleFor(tenant => tenant.Name)
                .NotEmpty()
                .WithState(t =>
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidValue,
                        ScimErrorDetail.AttributeRequired("name")));
        }

        protected override void ConfigureCreateRuleSet()
        {
        }

        protected override void ConfigureUpdateRuleSet()
        {
        }
    }
}