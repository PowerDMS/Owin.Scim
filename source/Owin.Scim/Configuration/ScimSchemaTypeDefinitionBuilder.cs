namespace Owin.Scim.Configuration
{
    public class ScimSchemaTypeDefinitionBuilder<T> : ScimTypeDefinitionBuilder<T>, IScimSchemaTypeDefinition
    {
        public ScimSchemaTypeDefinitionBuilder(string schema)
        {
            Schema = schema;
        }

        public string Schema { get; private set; }
    }
}