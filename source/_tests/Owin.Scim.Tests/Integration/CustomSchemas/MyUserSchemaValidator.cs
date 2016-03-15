namespace Owin.Scim.Tests.Integration.CustomSchemas
{
    using Model.Users;
    using Scim.Validation;

    public class MyUserSchemaValidator : ResourceExtensionValidatorBase<User, MyUserSchema>
    {
        protected override void ConfigureDefaultRuleSet()
        {
        }

        protected override void ConfigureCreateRuleSet()
        {
        }

        protected override void ConfigureUpdateRuleSet()
        {
        }

        public override string ExtensionSchema => MyUserSchema.Schema;
    }
}