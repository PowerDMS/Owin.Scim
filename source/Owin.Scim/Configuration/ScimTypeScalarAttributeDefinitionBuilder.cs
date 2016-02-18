namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public class ScimTypeScalarAttributeDefinitionBuilder<T, TMember> 
        : ScimTypeAttributeDefinitionBuilder<T, TMember>
    {
        public ScimTypeScalarAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor descriptor)
            : base(scimTypeDefinitionBuilder, descriptor)
        {
        }
    }
}