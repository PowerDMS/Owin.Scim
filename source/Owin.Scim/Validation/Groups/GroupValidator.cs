namespace Owin.Scim.Validation.Groups
{
    using Model.Groups;

    public class GroupValidator : ResourceValidatorBase<Group>
    {
        public GroupValidator(
            ResourceExtensionValidators extensionValidators)
            : base(extensionValidators)
        {
            
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