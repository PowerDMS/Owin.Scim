namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public class ScimTypeScalarAttributeDefinitionBuilder<T, TAttribute> 
        : ScimTypeAttributeDefinitionBuilder<T, TAttribute>
    {
        public ScimTypeScalarAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> typeDefinition,
            PropertyDescriptor propertyDescriptor,
            bool multiValued = false)
            : base(typeDefinition, propertyDescriptor, multiValued)
        {
        }

        internal ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetCaseExactInternal(bool caseExact)
        {
            CaseExact = caseExact;
            return this;
        }
    }
}