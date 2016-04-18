namespace Owin.Scim.Model
{
    using Configuration;

    public class AuthenticationSchemeDefinition : ScimTypeDefinitionBuilder<AuthenticationScheme>
    {
        public AuthenticationSchemeDefinition()
        {
            For(s => s.Type)
                .SetDescription(@"The authentication scheme.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(s => s.Name)
                .SetDescription(@"The common authentication scheme name, e.g., HTTP Basic.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(s => s.Description)
                .SetDescription(@"A description of the authentication scheme.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(s => s.SpecUri)
                .SetDescription(@"An HTTP-addressable URL pointing to the authentication scheme's specification.")
                .SetMutability(Mutability.ReadOnly)
                .SetReferenceTypes(ScimConstants.ReferenceTypes.External);

            For(s => s.DocumentationUri)
                .SetDescription(@"An HTTP-addressable URL pointing to the authentication scheme's usage documentation.")
                .SetMutability(Mutability.ReadOnly)
                .SetReferenceTypes(ScimConstants.ReferenceTypes.External);
        }
    }
}