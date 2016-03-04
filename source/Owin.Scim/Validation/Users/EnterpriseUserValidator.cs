namespace Owin.Scim.Validation.Users
{
    using Model.Users;

    using Repository;

    public class EnterpriseUserExtensionValidator : ResourceExtensionValidatorBase<User, EnterpriseUserExtension>
    {
        private readonly IUserRepository _UserRepository;

        public EnterpriseUserExtensionValidator(IUserRepository userRepository)
        {
            _UserRepository = userRepository;
        }

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