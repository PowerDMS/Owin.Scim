namespace Owin.Scim.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using Configuration;

    using Model;

    public static class IScimTypeAttributeDefinitionExtensions
    {
        public static ScimAttributeSchema ToScimAttributeSchema(this IScimTypeAttributeDefinition definition)
        {
            var subAttributes = new List<ScimAttributeSchema>();

            var definitionType = definition.GetType();
            if (definitionType.IsGenericType &&
                definitionType.GetGenericTypeDefinition() == typeof (ScimTypeComplexAttributeDefinitionBuilder<,>))
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
        
        private static ScimDataType GetScimDataType(PropertyDescriptor descriptor)
        {
            if (descriptor.PropertyType == typeof(string))
                return ScimDataType.String;

            if (descriptor.PropertyType == typeof(Uri))
                return ScimDataType.Reference;

            if (descriptor.PropertyType == typeof(bool) || descriptor.PropertyType == typeof(bool?))
                return ScimDataType.Boolean;

            if (descriptor.PropertyType == typeof(DateTime) ||
                descriptor.PropertyType == typeof(DateTime?) ||
                descriptor.PropertyType == typeof(DateTimeOffset) ||
                descriptor.PropertyType == typeof(DateTimeOffset?))
                return ScimDataType.DateTime;

            if (descriptor.PropertyType == typeof(decimal) || descriptor.PropertyType == typeof(decimal?))
                return ScimDataType.Decimal;

            if (descriptor.PropertyType == typeof(int) || descriptor.PropertyType == typeof(int?))
                return ScimDataType.Integer;

            if (!descriptor.PropertyType.IsTerminalObject())
                return ScimDataType.Complex;

            throw new ArgumentOutOfRangeException(
                "SCIM specification requires that all attributes data types must be of the following " +
                "type: \"string\", \"boolean\", \"decimal\", \"integer\", \"dateTime\", \"reference\", or \"complex\".");
        }
    }
}