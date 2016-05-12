namespace Owin.Scim.Model.Users
{
    using Canonicalization;
    using Configuration;

    public class InstantMessagingAddressDefinition : ScimTypeDefinitionBuilder<InstantMessagingAddress>
    {
        public InstantMessagingAddressDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(ima => ima.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);
            
            For(ima => ima.Value)
                .SetDescription(@"Instant messaging address for the user.");

            For(ima => ima.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'aim', 'gtalk', 'xmpp'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.InstantMessagingProviders)
                .AddCanonicalizationRule(type => type.ToLower());

            For(ima => ima.Primary)
                .SetDescription(
                    @"A Boolean value indicating the 'primary' or preferred attribute value 
                      for this attribute, e.g., the preferred messenger or primary messenger.");

            For(ima => ima.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}