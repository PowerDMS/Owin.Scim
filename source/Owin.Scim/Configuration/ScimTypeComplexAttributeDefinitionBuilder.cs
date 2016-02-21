namespace Owin.Scim.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>
        : ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute>
        where TComplexAttribute : class
    {
        private readonly ScimTypeDefinitionBuilder<TComplexAttribute> _TypeDefinitionBuilder;
        
        public ScimTypeComplexAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor propertyDescriptor,
            bool multiValued = false)
            : base (scimTypeDefinitionBuilder, propertyDescriptor)
        {
            MultiValued = multiValued;
            _TypeDefinitionBuilder = new ScimTypeDefinitionBuilder<TComplexAttribute>(ScimTypeDefinitionBuilder.ScimServerConfiguration);
        }

        public override IScimTypeDefinition TypeDefinitionBuilder
        {
            get { return _TypeDefinitionBuilder; }
        }

        protected internal IDictionary<PropertyDescriptor, IScimTypeAttributeDefinition> SubAttributeDefinitions
        {
            get { return _TypeDefinitionBuilder.AttributeDefinitions; }
        }
    }
}