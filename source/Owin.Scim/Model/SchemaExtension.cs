namespace Owin.Scim.Model
{
    using Newtonsoft.Json;

    [ScimTypeDefinition(typeof(SchemaExtensionDefinition))]
    public sealed class SchemaExtension
    {
        public SchemaExtension(string schema, bool required)
        {
            Schema = schema;
            Required = required;
        }

        [JsonProperty("schema")]
        public string Schema { get; private set; }

        [JsonProperty("required")]
        public bool Required { get; private set; }
    }
}