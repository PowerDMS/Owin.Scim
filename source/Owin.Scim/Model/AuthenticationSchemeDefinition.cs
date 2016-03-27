namespace Owin.Scim.Model
{
    using Configuration;

    public class AuthenticationSchemeDefinition : ScimTypeDefinitionBuilder<AuthenticationScheme>
    {
        public AuthenticationSchemeDefinition()
        {
            For(s => s.Display)
                .SetMutability(Mutability.ReadOnly);
            For(s => s.Name)
                .SetMutability(Mutability.ReadOnly);
            For(s => s.Description)
                .SetMutability(Mutability.ReadOnly);
            For(s => s.SpecUri)
                .SetMutability(Mutability.ReadOnly);
            For(s => s.DocumentationUri)
               .SetMutability(Mutability.ReadOnly);
        }
    }
}