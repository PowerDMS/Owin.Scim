namespace Owin.Scim.Model.Users
{
    using System;

    using Configuration;

    public class PhoneNumberDefinition : MultiValuedAttributeDefinition
    {
        public PhoneNumberDefinition( ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(mva => mva.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);
            
            For(p => p.Value)
                .SetDescription("Phone number of the user.");

            For(p => p.Type)
                .SetDescription(@"A label indicating the attribute's function, e.g., 'work', 'home', 'mobile'.")
                .SetCanonicalValues(ScimConstants.CanonicalValues.PhoneNumberTypes)
                .AddCanonicalizationRule(type => type.ToLower());

            For(p => p.Primary)
                .SetDescription(
                    @"A boolean value indicating the 'primary' or preferred attribute value for 
                      this attribute, e.g., the preferred phone number or primary phone number.");
        }

        public override Type DefinitionType
        {
            get { return typeof(PhoneNumber); }
        }
    }
}