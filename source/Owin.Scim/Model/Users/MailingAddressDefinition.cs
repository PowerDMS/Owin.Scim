namespace Owin.Scim.Model.Users
{
    using Canonicalization;
    using Configuration;

    public class MailingAddressDefinition : ScimTypeDefinitionBuilder<MailingAddress>
    {
        public MailingAddressDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(address => address.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);
            
            For(address => address.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work' or 'home'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.AddressTypes)
                .AddCanonicalizationRule(type => type.ToLower());

            For(address => address.Primary)
                .SetDescription(@"A boolean value indicating the 'primary' or preferred attribute value for this attribute.");

            For(address => address.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}