namespace Owin.Scim.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;
    
    using NContext.Extensions;

    public class ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>
        : ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute>
        where TComplexAttribute : class
    {
        private readonly ScimTypeDefinitionBuilder<TComplexAttribute> _TypeDefinitionBuilder;
        
        public ScimTypeComplexAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor descriptor,
            bool multiValued = false)
            : base (scimTypeDefinitionBuilder, descriptor)
        {
            MultiValued = multiValued;
            _TypeDefinitionBuilder = new ScimTypeDefinitionBuilder<TComplexAttribute>(ScimTypeDefinitionBuilder.ScimServerConfiguration);
        }

        protected internal ScimTypeDefinitionBuilder<TComplexAttribute> TypeDefinitionBuilder
        {
            get { return _TypeDefinitionBuilder; }
        }

        protected internal IDictionary<PropertyDescriptor, IScimTypeAttributeDefinitionBuilder> SubAttributeDefinitions
        {
            get { return _TypeDefinitionBuilder.AttributeDefinitions; }
        }
    }
}