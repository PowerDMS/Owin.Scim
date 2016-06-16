namespace Owin.Scim.Model
{
    using Configuration;

    using v2;
    using v2.Model;

    public sealed class ScimSchema2TypeDefinition : ScimSchemaTypeDefinitionBuilder<ScimSchema2>
    {
        public ScimSchema2TypeDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration, ScimConstantsV2.Schemas.Schema)
        {
            SetName(ScimConstants.ResourceTypes.Schema);
            SetDescription(@"Specifies the schema that describes a SCIM schema.");

            For(s => s.Id)
                .SetDescription(@"The unique URI of the schema.")
                .SetMutability(Mutability.ReadOnly);

            For(s => s.Name)
                .SetDescription(@"The schema's human-readable name. When applicable, service providers MUST specify the name, e.g., 'User'.")
                .SetMutability(Mutability.ReadOnly);

            For(s => s.Description)
                .SetDescription(@"The schema's human-readable description. When applicable, service providers MUST specify the description.")
                .SetMutability(Mutability.ReadOnly);

            For(s => s.Attributes)
                .SetDescription(@"A complex attribute that includes the attributes of a schema.")
                .SetMutability(Mutability.ReadOnly);
        }
    }
}