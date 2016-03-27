namespace Owin.Scim.Model
{
    using Configuration;

    public class ResourceTypeDefinition : ScimTypeDefinitionBuilder<ResourceType>
    {
        public ResourceTypeDefinition()
        {
            For(u => u.Schemas)
                .SetReturned(Returned.Always);

            For(rt => rt.Id)
                .SetReturned(Returned.Never);

            For(rt => rt.ExternalId)
                .SetReturned(Returned.Never);
        }
    }
}