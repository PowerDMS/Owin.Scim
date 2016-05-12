namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class EntitlementDefinition : ScimTypeDefinitionBuilder<Entitlement>
    {
        public EntitlementDefinition( ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(entitlement => entitlement.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);

            For(entitlement => entitlement.Type)
                .SetDescription("A label indicating the attribute's function, e.g., 'work' or 'home'.");

            For(entitlement => entitlement.Primary)
                .SetDescription(@"A boolean value indicating the 'primary' or preferred attribute value for this attribute.");

            For(entitlement => entitlement.Value)
                .SetDescription("The value of an entitlement.");

            For(entitlement => entitlement.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}