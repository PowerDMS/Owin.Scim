namespace Owin.Scim.Validation.Users
{
    using Model.Users;

    public class EnterpriseUserExtensionValidator : ResourceExtensionValidatorBase<User, EnterpriseUserExtension>
    {
        public override string ExtensionSchema
        {
            get { return ScimConstants.Schemas.UserEnterprise; }
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