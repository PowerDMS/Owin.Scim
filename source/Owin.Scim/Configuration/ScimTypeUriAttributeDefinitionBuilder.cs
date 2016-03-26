namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ScimTypeUriAttributeDefinitionBuilder<T, TMember>
        : ScimTypeScalarAttributeDefinitionBuilder<T, TMember>
        where TMember : Uri
    {
        public ScimTypeUriAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> typeDefinition,
            PropertyDescriptor propertyDescriptor)
            : base(typeDefinition, propertyDescriptor)
        {
        }

        internal IEnumerable<string> ReferenceTypes { get; set; } 

        internal void AddReferenceTypes(IEnumerable<string> referenceTypes)
        {
            ReferenceTypes = referenceTypes;
        }
    }
}