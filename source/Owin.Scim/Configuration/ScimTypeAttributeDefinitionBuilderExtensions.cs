namespace Owin.Scim.Configuration
{
    using System;
    using System.Linq;

    using Extensions;

    using Model;

    public static class ScimTypeAttributeDefinitionBuilderExtensions
    {
        public static ScimTypeAttributeDefinitionBuilder<T, string> SetCaseExact<T>(
            this ScimTypeAttributeDefinitionBuilder<T, string> attributeBuilder,
            bool caseExact)
        {
            var stringBuilder = (ScimTypeScalarAttributeDefinitionBuilder<T, string>)attributeBuilder;

            stringBuilder.SetCaseExactInternal(caseExact);

            return stringBuilder;
        }

        public static ScimTypeAttributeDefinitionBuilder<T, TUri> SetReferenceTypes<T, TUri>(
            this ScimTypeAttributeDefinitionBuilder<T, TUri> attributeBuilder,
            params string[] referenceTypes)
            where TUri : Uri
        {
            var uriBuilder = attributeBuilder as ScimTypeUriAttributeDefinitionBuilder<T, TUri>;
            if (uriBuilder == null) throw new InvalidOperationException("You cannot define reference types on a non-Uri attribute type.");
            
            uriBuilder.AddReferenceTypes(referenceTypes.ToList());

            return uriBuilder;
        }

        public static ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute> DefineSubAttributes<T, TComplexAttribute>(
            this ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute> attributeBuilder,
            Action<ScimTypeDefinitionBuilder<TComplexAttribute>> builder)
            where TComplexAttribute : class
        {
            if (!(attributeBuilder is ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>))
                throw new InvalidOperationException("You cannot define sub-attributes on a non-complex attribute type.");
            
            builder((ScimTypeDefinitionBuilder<TComplexAttribute>)attributeBuilder.TypeDefinition);
            
            return attributeBuilder;
        }
    }
}