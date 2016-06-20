namespace Owin.Scim.v1.Model
{
    using Configuration;

    public sealed class ScimSchema1TypeDefinition : ScimSchemaTypeDefinitionBuilder<ScimSchema1>
    {
        public ScimSchema1TypeDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration, ScimConstantsV1.Schemas.Schema)
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