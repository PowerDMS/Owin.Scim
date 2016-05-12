namespace Owin.Scim.Tests.Integration.SchemaExtensions
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

        public override string ExtensionSchema
        {
            get { return MyGroupSchema.Schema; }
        }
    }
}