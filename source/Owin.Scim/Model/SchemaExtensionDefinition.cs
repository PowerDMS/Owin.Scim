namespace Owin.Scim.Model
{
    using Configuration;

    public class SchemaExtensionDefinition : ScimTypeDefinitionBuilder<SchemaExtension>
    {
        public SchemaExtensionDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
            For(se => se.Required)
                .SetDescription(@"A boolean value that specifies whether or not the schema extension is required for the resource type.")
                .SetRequired(true);

            For(se => se.Schema)
                .SetDescription(
                    @"The URI of an extended schema, e.g., ""urn: edu:2.0:Staff"". This MUST be equal 
                      to the ""id"" attribute of a ""Schema"" resource.")
                .SetRequired(true);
        }
    }
}