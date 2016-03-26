namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public class ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>
        : ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute>
        where TComplexAttribute : class
    {
        public ScimTypeComplexAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> typeDefinition,
            PropertyDescriptor propertyDescriptor,
            bool multiValued = false)
            : base (typeDefinition, propertyDescriptor)
        {
            MultiValued = multiValued;
        }

        public override IScimTypeDefinition DeclaringTypeDefinition
        {
            get { return ScimServerConfiguration.GetScimTypeDefinition(typeof(TComplexAttribute)); }
        }
    }
}