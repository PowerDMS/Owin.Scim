namespace Owin.Scim.Model
{
    using Configuration;

    public class ResourceMetadataDefinition : ScimTypeDefinitionBuilder<ResourceMetadata>
    {
        public ResourceMetadataDefinition()
        {
            For(m => m.ResourceType)
                .SetMutability(Mutability.ReadOnly)
                .SetCaseExact(true);
            For(m => m.Created)
                .SetMutability(Mutability.ReadOnly);
            For(m => m.LastModified)
                .SetMutability(Mutability.ReadOnly);
            For(m => m.Location)
                .SetMutability(Mutability.ReadOnly);
            For(m => m.Version)
                .SetMutability(Mutability.ReadOnly)
                .SetCaseExact(true);
        }
    }
}