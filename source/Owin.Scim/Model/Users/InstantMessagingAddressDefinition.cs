namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class InstantMessagingAddressDefinition : MultiValuedAttributeDefinition
    {
        public InstantMessagingAddressDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
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
        }

        public override Type DefinitionType
        {
            get { return typeof (InstantMessagingAddress); }
        }
    }
}