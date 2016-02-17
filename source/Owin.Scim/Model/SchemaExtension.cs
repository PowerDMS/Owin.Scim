namespace Owin.Scim.Model
{
    public sealed class SchemaExtension
    {
        public SchemaExtension(string schema, bool required)
        {
            Schema = schema;
            Required = required;
        }

        public string Schema { get; private set; }

        public bool Required { get; private set; }
    }
}