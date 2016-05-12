namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class EmailDefinition : ScimTypeDefinitionBuilder<Email>
    {
        public EmailDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(email => email.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);
            
            For(email => email.Value)
                .SetDescription("Email address value.");

            For(email => email.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work' or 'home'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.EmailAddressTypes, StringComparer.OrdinalIgnoreCase)
                .AddCanonicalizationRule(type => type.ToLower());

            For(email => email.Primary)
                .SetDescription(
                    @"A Boolean value indicating the 'primary' or preferred attribute value 
                      for this attribute, e.g., the preferred mailing address or primary email address.");

            For(email => email.Ref)
                .AddCanonicalizationRule((uri, definition) => Canonicalization.Canonicalization.EnforceScimUri(uri, definition, ServerConfiguration));
        }
    }
}