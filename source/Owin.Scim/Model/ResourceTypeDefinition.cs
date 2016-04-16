namespace Owin.Scim.Model
{
    using Configuration;

    public class ResourceTypeDefinition : ScimSchemaTypeDefinitionBuilder<ResourceType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTypeDefinition"/> class.
        /// </summary>
        public ResourceTypeDefinition()
            : base(ScimConstants.Schemas.ResourceType)
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