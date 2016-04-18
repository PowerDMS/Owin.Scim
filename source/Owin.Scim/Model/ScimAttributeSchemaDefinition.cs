namespace Owin.Scim.Model
{
    using Configuration;

    public class ScimAttributeSchemaDefinition : ScimTypeDefinitionBuilder<ScimAttributeSchema>
    {
        public ScimAttributeSchemaDefinition()
        {
            For(a => a.Name)
                .SetDescription(@"The attribute's name.")
                .SetMutability(Mutability.ReadOnly)
                .SetCaseExact(true)
                .SetRequired(true);

            For(a => a.Type)
                .SetDescription(
                    @"The attribute's data type. Valid values include 
                      'string', 'complex', 'boolean', 'decimal', 'integer', 'dateTime', 'reference', 'binary'.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true)
                .SetCanonicalValues(ScimConstants.CanonicalValues.ScimDataTypes);

            For(a => a.MultiValued)
                .SetDescription(@"A boolean value indicating an attribute's plurality.")
                .SetMutability(Mutability.ReadOnly)
                .SetRequired(true);

            For(a => a.Description)
                .SetDescription(@"A human-readable description of the attribute.")
                .SetMutability(Mutability.ReadOnly);

            For(a => a.Required)
                .SetDescription(@"A Boolean value that specifies whether or not the attribute is required.")
                .SetMutability(Mutability.ReadOnly);

            For(a => a.CanonicalValues)
                .SetDescription(
                    @"A collection of suggested canonical values that MAY be used (e.g., ""work"" and ""home""). 
                      In some cases, service providers MAY choose to ignore unsupported values.")
                .SetMutability(Mutability.ReadOnly);

            For(a => a.CaseExact)
                .SetDescription(@"A boolean value that specifies whether or not a string attribute is case sensitive.")
                .SetMutability(Mutability.ReadOnly);

            For(a => a.Mutability)
                .SetDescription(@"A single keyword indicating the circumstances under which the value of the attribute can be (re)defined.")
                .SetMutability(Mutability.ReadOnly)
                .SetCanonicalValues(ScimConstants.CanonicalValues.ScimMutabilityOptions);

            For(a => a.Returned)
                .SetDescription(
                    @"A single keyword that indicates when an attribute and associated values are returned in 
                      response to a GET request or in response to a PUT, POST, or PATCH request.")
                .SetMutability(Mutability.ReadOnly)
                .SetCanonicalValues(ScimConstants.CanonicalValues.ScimReturnedOptions);

            For(a => a.Uniqueness)
                .SetDescription(@"A single keyword value that specifies how the service provider enforces uniqueness of attribute values.")
                .SetMutability(Mutability.ReadOnly)
                .SetCanonicalValues(ScimConstants.CanonicalValues.ScimUniquenessOptions);

            For(a => a.ReferenceTypes)
                .SetDescription(@"A multi-valued array of JSON strings that indicate the SCIM resource types that may be referenced.")
                .SetMutability(Mutability.ReadOnly)
                .SetCanonicalValues(ScimConstants.CanonicalValues.ScimReferenceOptions);

            For(a => a.SubAttributes)
                .SetDescription(@"Used to define the sub-attributes of a complex attribute.")
                .SetMutability(Mutability.ReadOnly);
        }
    }
}