namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public class ScimTypeScalarAttributeDefinitionBuilder<T, TAttribute> 
        : ScimTypeAttributeDefinitionBuilder<T, TAttribute>
    {
        public ScimTypeScalarAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> typeDefinition,
            PropertyDescriptor propertyDescriptor)
            : base(typeDefinition, propertyDescriptor)
        {
        }

        internal ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetCaseExactInternal(bool caseExact)
        {
            CaseExact = caseExact;
            return this;
        }
    }
}