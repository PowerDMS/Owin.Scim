namespace Owin.Scim.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public class ScimTypeUriAttributeDefinitionBuilder<T, TAttribute>
        : ScimTypeScalarAttributeDefinitionBuilder<T, TAttribute>
        where TAttribute : Uri
    {
        public ScimTypeUriAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> typeDefinition,
            PropertyDescriptor propertyDescriptor,
            bool multiValued = false)
            : base(typeDefinition, propertyDescriptor, multiValued)
        {
        }

        internal ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetReferenceTypesInternal(IEnumerable<string> referenceTypes)
        {
            ReferenceTypes = referenceTypes.ToList();
            return this;
        }
    }
}