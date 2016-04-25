namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class EnterpriseUserExtensionDefinition : ScimTypeDefinitionBuilder<EnterpriseUserExtension>
    {
        public EnterpriseUserExtensionDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            SetName("Enterprise user");
            SetDescription("Enterprise user resource extension.");

            For(e => e.EmployeeNumber)
                .SetDescription(
                    @"A string identifier, typically numeric or alphanumeric, assigned to a person, 
                      typically based on order of hire or association with an organization.");

            For(e => e.CostCenter)
                .SetDescription(@"Identifies the name of a cost center.");

            For(e => e.Organization)
                .SetDescription(@"Identifies the name of an organization.");

            For(e => e.Division)
                .SetDescription(@"Identifies the name of a division.");

            For(e => e.Department)
                .SetDescription(@"Identifies the name of a department.");

            For(e => e.Manager)
                .SetDescription(@"The user's manager.");
        }
    }
}