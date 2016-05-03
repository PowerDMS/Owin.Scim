namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class RoleDefinition : ScimTypeDefinitionBuilder<Role>
    {
        public RoleDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(mva => mva.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);

            For(mva => mva.Type)
                .SetDescription("A label indicating the attribute's function, e.g., 'work' or 'home'.");

            For(mva => mva.Primary)
                .SetDescription(@"A boolean value indicating the 'primary' or preferred attribute value for this attribute.");

            For(e => e.Value)
                .SetDescription("The value of a role.");
        }
    }
}