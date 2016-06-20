namespace Owin.Scim.v2.Model
{
    using Configuration;

    public class ResourceTypeDefinition : ScimSchemaTypeDefinitionBuilder<ResourceType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTypeDefinition"/> class.
        /// </summary>
        public ResourceTypeDefinition(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration, ScimConstantsV2.Schemas.ResourceType)
        {
            SetName(ScimConstants.ResourceTypes.ResourceType);
            SetDescription("Specifies the schema that describes a SCIM resource type.");

            For(rt => rt.Schemas)
                .SetReturned(Returned.Always);

            For(rt => rt.Id)
                .SetDescription(@"The resource type's server unique id. This is often the same value as the ""name"" attribute.")
                .SetReturned(Returned.Never)
                .SetMutability(Mutability.ReadOnly);

            For(rt => rt.ExternalId)
                .SetReturned(Returned.Never);

            For(rt => rt.Name)
                .SetDescription(@"The resource type name. When applicable, service providers MUST specify the name, e.g., 'User'.")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);

            For(rt => rt.Description)
                .SetDescription(@"The resource type's human-readable description. When applicable, service providers MUST specify the description.")
                .SetMutability(Mutability.ReadOnly);

            For(rt => rt.Endpoint)
                .SetDescription(@"The resource type's HTTP-addressable endpoint relative to the Base URL of the service provider, e.g., ""Users"".")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);

            For(rt => rt.Schema)
                .SetDescription(@"The resource type's primary/base schema URI.")
                .SetRequired(true)
                .SetMutability(Mutability.ReadOnly);

            For(rt => rt.SchemaExtensions)
                .SetDescription(@"A list of URIs of the resource type's schema extensions.")
                .SetMutability(Mutability.ReadOnly);
        }
    }
}