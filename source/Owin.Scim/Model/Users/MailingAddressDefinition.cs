namespace Owin.Scim.Model.Users
{
    using System;

    public class MailingAddressDefinition : MultiValuedAttributeDefinition
    {
        public MailingAddressDefinition()
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