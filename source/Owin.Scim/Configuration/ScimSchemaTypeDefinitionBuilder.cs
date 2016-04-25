namespace Owin.Scim.Configuration
{
    public class ScimSchemaTypeDefinitionBuilder<T> : ScimTypeDefinitionBuilder<T>, IScimSchemaTypeDefinition
    {
        public ScimSchemaTypeDefinitionBuilder(ScimServerConfiguration serverConfiguration, string schema) 
            : base(serverConfiguration)
        {
            Schema = schema;
        }

        public string Schema { get; private set; }
    }
}