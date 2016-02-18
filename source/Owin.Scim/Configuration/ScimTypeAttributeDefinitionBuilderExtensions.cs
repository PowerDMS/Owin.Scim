namespace Owin.Scim.Configuration
{
    using System;

    public static class ScimTypeAttributeDefinitionBuilderExtensions
    {
        public static ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute> ForSubAttributes<T, TComplexAttribute>(
            this ScimTypeAttributeDefinitionBuilder<T, TComplexAttribute> attributeBuilder,
            Action<ScimTypeDefinitionBuilder<TComplexAttribute>> builder)
            where TComplexAttribute : class
        {
            var complexBuilder = attributeBuilder as ScimTypeComplexAttributeDefinitionBuilder<T, TComplexAttribute>;
            if (complexBuilder == null) throw new InvalidOperationException("You cannot define sub-attributes on a non-complex attribute type.");

            var typeBuilder = new ScimTypeDefinitionBuilder<TComplexAttribute>(
                attributeBuilder.ScimTypeDefinitionBuilder.ScimServerConfiguration);

            builder(typeBuilder);
            
            complexBuilder.AddSubAttributeDefinitions(typeBuilder.MemberDefinitions); // TODO: (DG) Change to dictionary?

            return complexBuilder;
        }
    }
}