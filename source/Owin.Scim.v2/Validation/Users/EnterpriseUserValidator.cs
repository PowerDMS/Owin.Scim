namespace Owin.Scim.v2.Validation.Users
{
    using Model;

    using Scim.Model.Users;
    using Scim.Validation;

    public class EnterpriseUser2ExtensionValidator : ResourceExtensionValidatorBase<ScimUser, EnterpriseUser2Extension>
    {
        public override string ExtensionSchema
        {
            get { return ScimConstantsV2.Schemas.UserEnterprise; }
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