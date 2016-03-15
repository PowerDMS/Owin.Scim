namespace Owin.Scim.Tests.Integration.CustomSchemas
{
    using Model.Groups;
    using Scim.Validation;

    public class MyGroupSchemaValidator : ResourceExtensionValidatorBase<Group, MyGroupSchema>
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

        public override string ExtensionSchema => MyGroupSchema.Schema;
    }
}