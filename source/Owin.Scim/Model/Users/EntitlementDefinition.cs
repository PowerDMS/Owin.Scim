namespace Owin.Scim.Model.Users
{
    using System;

    public class EntitlementDefinition : MultiValuedAttributeDefinition
    {
        public EntitlementDefinition()
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