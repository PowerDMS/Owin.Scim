namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class EmailDefinition : MultiValuedAttributeDefinition
    {
        public EmailDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(e => e.Value)
                .SetDescription("Email address value.");

            For(e => e.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work' or 'home'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.EmailAddressTypes, StringComparer.OrdinalIgnoreCase)
                .AddCanonicalizationRule(type => type.ToLower());

            For(e => e.Primary)
                .SetDescription(
                    @"A Boolean value indicating the 'primary' or preferred attribute value 
                      for this attribute, e.g., the preferred mailing address or primary email address.");
        }

        public override Type DefinitionType
        {
            get { return typeof (Email); }
        }
    }
}