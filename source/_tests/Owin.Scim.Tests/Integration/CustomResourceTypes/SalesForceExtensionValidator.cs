namespace Owin.Scim.Tests.Integration.CustomResourceTypes
{
    using System.Net;

    using ErrorHandling;

    using FluentValidation;

    using Model;

    using Scim.Validation;

    public class SalesForceExtensionValidator : ResourceExtensionValidatorBase<Tenant, SalesForceExtension>
    {
        public override string ExtensionSchema
        {
            get { return CustomSchemas.SalesForceExtension; }
        }

        protected override void ConfigureDefaultRuleSet()
        {
            RuleFor(sfe => sfe.CustomerIdentifier)
                .NotEmpty()
                .WithState(e => 
                    new ScimError(
                        HttpStatusCode.BadRequest,
                        ScimErrorType.InvalidValue,
                        ScimErrorDetail.AttributeRequired("customerIdentifier")));
        }

        protected override void ConfigureCreateRuleSet()
        {
        }

        protected override void ConfigureUpdateRuleSet()
        {
        }
    }
}