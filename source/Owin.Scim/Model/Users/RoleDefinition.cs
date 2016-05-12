namespace Owin.Scim.Model.Users
{
    using Canonicalization;
    using Configuration;

    public class RoleDefinition : ScimTypeDefinitionBuilder<Role>
    {
        public RoleDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(role => role.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);

            For(role => role.Type)
                .SetDescription("A label indicating the attribute's function, e.g., 'work' or 'home'.");

            For(role => role.Primary)
                .SetDescription(@"A boolean value indicating the 'primary' or preferred attribute value for this attribute.");

            For(role => role.Value)
                .SetDescription("The value of a role.");

            For(role => role.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}