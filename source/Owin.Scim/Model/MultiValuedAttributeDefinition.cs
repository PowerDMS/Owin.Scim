namespace Owin.Scim.Model
{
    using Configuration;

    public class MultiValuedAttributeDefinition : ScimTypeDefinitionBuilder<MultiValuedAttribute>
    {
        public MultiValuedAttributeDefinition()
        {
            For(e => e.Display)
                .SetMutability(Mutability.ReadOnly);
        }
    }
}