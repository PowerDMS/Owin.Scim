namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public class ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>
        : ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute>
        where TComplexAttribute : class
    {
        public ScimTypeComplexAttributeDefinitionBuilder(
            IScimTypeDefinition typeDefinition,
            PropertyDescriptor propertyDescriptor,
            bool multiValued = false)
            : base (typeDefinition, propertyDescriptor, multiValued)
        {
        }
    }
}