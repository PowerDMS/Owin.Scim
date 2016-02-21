namespace Owin.Scim.Configuration
{
    using System.ComponentModel;

    public class ScimTypeScalarAttributeDefinitionBuilder<T, TAttribute> 
        : ScimTypeAttributeDefinitionBuilder<T, TAttribute>
    {
        public ScimTypeScalarAttributeDefinitionBuilder(
            ScimTypeDefinitionBuilder<T> scimTypeDefinitionBuilder,
            PropertyDescriptor propertyDescriptor)
            : base(scimTypeDefinitionBuilder, propertyDescriptor)
        {
        }

        internal ScimTypeAttributeDefinitionBuilder<T, TAttribute> SetCaseExactInternal(bool caseExact)
        {
            CaseExact = caseExact;
            return this;
        }
    }
}