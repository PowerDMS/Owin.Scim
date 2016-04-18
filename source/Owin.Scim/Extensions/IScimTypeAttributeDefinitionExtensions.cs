namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    using Configuration;

    using Model;

    public static class IScimTypeAttributeDefinitionExtensions
    {
        public static ScimAttributeSchema ToScimAttributeSchema(this IScimTypeAttributeDefinition definition)
        {
            var subAttributes = new List<ScimAttributeSchema>();

            var definitionType = definition.GetType();
            Type[] genericTypeArguments;
            if (definitionType.IsGenericType &&
                definitionType.GetGenericTypeDefinition() == typeof (ScimTypeComplexAttributeDefinitionBuilder<,>) &&
                (genericTypeArguments = definitionType.GetGenericArguments())[0] != genericTypeArguments[1]) // circular reference check e.g <ScimAttributeSchema, ScimAttributeSchema>
            {
                foreach (var subAttDefinition in definition.DeclaringTypeDefinition.AttributeDefinitions.Values)
                {
                    subAttributes.Add(subAttDefinition.ToScimAttributeSchema());
                }
            }

            return new ScimAttributeSchema(
                definition.Name,
                GetScimDataType(definition.AttributeDescriptor),
                definition.Description,
                definition.MultiValued,
                definition.Mutability.ToString().LowercaseFirstCharacter(),
                definition.Required,
                definition.Returned.ToString().LowercaseFirstCharacter(),
                definition.Uniqueness.ToString().LowercaseFirstCharacter(),
                definition.CaseExact,
                subAttributes,
                definition.CanonicalValues,
                definition.ReferenceTypes);
        }
        
        private static string GetScimDataType(PropertyDescriptor descriptor)
        {
            var attributeType = descriptor.PropertyType;
            if (attributeType.IsNonStringEnumerable())
                attributeType = descriptor.PropertyType.IsArray
                    ? descriptor.PropertyType.GetElementType()
                    : descriptor.PropertyType.GetGenericArguments()[0];

            if (attributeType == typeof(string))
                return ScimConstants.DataTypes.String;

            if (attributeType == typeof(Uri))
                return ScimConstants.DataTypes.Reference;

            if (attributeType == typeof(bool) || attributeType == typeof(bool?))
                return ScimConstants.DataTypes.Boolean;

            if (attributeType == typeof(DateTime) ||
                attributeType == typeof(DateTime?) ||
                attributeType == typeof(DateTimeOffset) ||
                attributeType == typeof(DateTimeOffset?))
                return ScimConstants.DataTypes.DateTime;

            if (attributeType == typeof(decimal) || attributeType == typeof(decimal?))
                return ScimConstants.DataTypes.Decimal;

            if (attributeType == typeof(int) || attributeType == typeof(int?))
                return ScimConstants.DataTypes.Integer;

            if (attributeType == typeof (byte) || attributeType == typeof (byte?) ||
                attributeType == typeof (byte[]) || typeof (Stream).IsAssignableFrom(attributeType))
                return ScimConstants.DataTypes.Binary;

            // you should avoid designing your classes with ambiguous data types like object
            // Owin.Scim only uses this for CanonicalValues
            if (attributeType == typeof (object))
                return descriptor.Name.Equals("CanonicalValues", StringComparison.OrdinalIgnoreCase)
                    ? "SCIM data-type of the associated attribute type being defined."
                    : ScimConstants.DataTypes.String;

            if (!attributeType.IsTerminalObject())
                return ScimConstants.DataTypes.Complex;

            throw new ArgumentOutOfRangeException(
                "SCIM specification requires that all attributes data types must be of the following " +
                "type: \"string\", \"boolean\", \"decimal\", \"integer\", \"dateTime\", \"reference\", or \"complex\".");
        }
    }
}