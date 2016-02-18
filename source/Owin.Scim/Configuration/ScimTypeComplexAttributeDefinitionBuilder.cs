namespace Owin.Scim.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;
    
    using NContext.Extensions;

    public class ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>
        : ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute>
        where TComplexAttribute : class
    {
        private readonly ISet<IScimTypeAttributeDefinitionBuilder> _SubAttributeDefinitions = 
            new HashSet<IScimTypeAttributeDefinitionBuilder>();
        
        public ScimTypeComplexAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor descriptor,
            bool multiValued = false)
            : base (scimTypeDefinitionBuilder, descriptor)
        {
            MultiValued = multiValued;
        }
        
        internal void AddSubAttributeDefinitions(IDictionary<PropertyDescriptor, IScimTypeAttributeDefinitionBuilder> memberDefinitions)
        {
            _SubAttributeDefinitions.AddRange(memberDefinitions.Values); // TODO: (DG) Change to dictionary?
        }
    }
}