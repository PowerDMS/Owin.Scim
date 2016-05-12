namespace Owin.Scim.Model.Users
{
    using Canonicalization;
    using Configuration;

    public class PhoneNumberDefinition : ScimTypeDefinitionBuilder<PhoneNumber>
    {
        public PhoneNumberDefinition( ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(phoneNumber => phoneNumber.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);
            
            For(phoneNumber => phoneNumber.Value)
                .SetDescription("Phone number of the user.");

            For(phoneNumber => phoneNumber.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work', 'home', 'mobile'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.PhoneNumberTypes)
                .AddCanonicalizationRule(type => type.ToLower());

            For(phoneNumber => phoneNumber.Primary)
                .SetDescription(
                    @"A boolean value indicating the 'primary' or preferred attribute value for 
                      this attribute, e.g., the preferred phone number or primary phone number.");

            For(phoneNumber => phoneNumber.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}