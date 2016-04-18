namespace Owin.Scim.Model
{
    using Configuration;

    public class ResourceMetadataDefinition : ScimTypeDefinitionBuilder<ResourceMetadata>
    {
        public ResourceMetadataDefinition()
        {
            For(m => m.ResourceType)
                .SetDescription(@"The name of the resource type of the resource.")
                .SetMutability(Mutability.ReadOnly)
                .SetCaseExact(true);

            For(m => m.Created)
                .SetDescription(@"The DateTime that the resource was added to the service provider.")
                .SetMutability(Mutability.ReadOnly);

            For(m => m.LastModified)
                .SetDescription(@"The most recent DateTime that the details of this resource were updated at the service provider.")
                .SetMutability(Mutability.ReadOnly);

            For(m => m.Location)
                .SetDescription(@"The URI of the resource being returned.")
                .SetMutability(Mutability.ReadOnly);

            For(m => m.Version)
                .SetDescription(@"The version of the resource being returned.")
                .SetMutability(Mutability.ReadOnly)
                .SetCaseExact(true);
        }
    }
}