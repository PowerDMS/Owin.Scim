namespace Owin.Scim.Model
{
    using Configuration;

    public sealed class ScimSchemaTypeDefinition : ScimSchemaTypeDefinitionBuilder<ScimSchema>
    {
        public ScimSchemaTypeDefinition() : base(ScimConstants.Schemas.Schema)
        {
        }
    }
}