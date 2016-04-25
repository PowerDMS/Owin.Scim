namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class MailingAddressDefinition : MultiValuedAttributeDefinition
    {
        public MailingAddressDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(a => a.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work' or 'home'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.AddressTypes)
                .AddCanonicalizationRule(type => type.ToLower());
        }

        public override Type DefinitionType
        {
            get { return typeof(MailingAddress); }
        }
    }
}