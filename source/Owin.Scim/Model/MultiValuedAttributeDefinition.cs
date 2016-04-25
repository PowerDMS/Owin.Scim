namespace Owin.Scim.Model
{
    using Configuration;

    public abstract class MultiValuedAttributeDefinition : ScimTypeDefinitionBuilder<MultiValuedAttribute>
    {
        protected MultiValuedAttributeDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(mva => mva.Display)
                .SetDescription("A human-readable name, primarily used for display purposes.")
                .SetMutability(Mutability.ReadOnly);

            For(mva => mva.Type)
                .SetDescription("A label indicating the attribute's function, e.g., 'work' or 'home'.");

            For(mva => mva.Primary)
                .SetDescription(@"A boolean value indicating the 'primary' or preferred attribute value for this attribute.");
        }
    }
}