namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class EntitlementDefinition : MultiValuedAttributeDefinition
    {
        public EntitlementDefinition( ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(e => e.Value)
                .SetDescription("The value of an entitlement.");

            For(e => e.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work' or 'home'.");
        }

        public override Type DefinitionType
        {
            get { return typeof(Entitlement); }
        }
    }
}