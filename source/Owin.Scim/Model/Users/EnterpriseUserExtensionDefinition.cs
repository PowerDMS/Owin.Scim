namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class EnterpriseUserExtensionDefinition : ScimTypeDefinitionBuilder<EnterpriseUserExtension>
    {
        public EnterpriseUserExtensionDefinition()
        {
            SetName("EnterpriseUser");
            SetDescription("Enterprise user.");
        }
    }
}