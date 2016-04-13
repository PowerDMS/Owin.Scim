namespace Owin.Scim.Configuration
{
    using System;
    using System.Linq;
    
    public static class ScimTypeAttributeDefinitionBuilderExtensions
    {
        public static ScimTypeAttributeDefinitionBuilder<T, string> SetCaseExact<T>(
            this ScimTypeAttributeDefinitionBuilder<T, string> attributeBuilder,
            bool caseExact)
        {
            var stringBuilder = attributeBuilder as ScimTypeScalarAttributeDefinitionBuilder<T, string>;
            if (stringBuilder == null) throw new InvalidOperationException("You cannot define caseExact on a non-string attribute type.");

            return stringBuilder.SetCaseExactInternal(caseExact);
        }

        public static ScimTypeAttributeDefinitionBuilder<T, TUri> SetReferenceTypes<T, TUri>(
            this ScimTypeAttributeDefinitionBuilder<T, TUri> attributeBuilder,
            params string[] referenceTypes)
            where TUri : Uri
        {
            var uriBuilder = attributeBuilder as ScimTypeUriAttributeDefinitionBuilder<T, TUri>;
            if (uriBuilder == null) throw new InvalidOperationException("You cannot define reference types on a non-Uri attribute type.");
            
            return uriBuilder.SetReferenceTypesInternal(referenceTypes.ToList());
        }
    }
}