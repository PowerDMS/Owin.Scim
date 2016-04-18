namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class EnterpriseUserExtensionDefinition : ScimTypeDefinitionBuilder<EnterpriseUserExtension>
    {
        public EnterpriseUserExtensionDefinition()
        {
            SetName("Enterprise user");
            SetDescription("Enterprise user resource extension.");
        }
    }
}