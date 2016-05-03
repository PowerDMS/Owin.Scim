namespace Owin.Scim.Model.Users
{
    using Configuration;

    public class MailingAddressDefinition : ScimTypeDefinitionBuilder<MailingAddress>
    {
        public MailingAddressDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(a => a.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);
            
            For(a => a.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work' or 'home'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.AddressTypes)
                .AddCanonicalizationRule(type => type.ToLower());

            For(a => a.Primary)
                .SetDescription(@"A boolean value indicating the 'primary' or preferred attribute value for this attribute.");
        }
    }
}