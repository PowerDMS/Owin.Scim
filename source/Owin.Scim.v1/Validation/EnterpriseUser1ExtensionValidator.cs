namespace Owin.Scim.v1.Validation
{
    using Model;

    using Scim.Model.Users;
    using Scim.Validation;

    public class EnterpriseUser1ExtensionValidator : ResourceExtensionValidatorBase<ScimUser, EnterpriseUser1Extension>
    {
        public override string ExtensionSchema
        {
            get { return ScimConstantsV1.Schemas.UserEnterprise; }
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
    }
}