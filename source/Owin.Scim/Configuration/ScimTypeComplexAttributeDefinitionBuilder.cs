namespace Owin.Scim.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>
        : ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute>
        where TComplexAttribute : class
    {
        private readonly ScimTypeDefinitionBuilder<TComplexAttribute> _TypeDefinition;
        
        public ScimTypeComplexAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor propertyDescriptor,
            bool multiValued = false)
            : base (scimTypeDefinitionBuilder, propertyDescriptor)
        {
            MultiValued = multiValued;
            _TypeDefinition = new ScimTypeDefinitionBuilder<TComplexAttribute>(scimTypeDefinitionBuilder.ScimServerConfiguration);
        }

        public override IScimTypeDefinition TypeDefinition
        {
            get { return _TypeDefinition; }
        }
    }
}