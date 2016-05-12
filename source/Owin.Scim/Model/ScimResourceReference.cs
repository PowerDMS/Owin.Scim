using Owin.Scim.Configuration;

namespace Owin.Scim.Model
{
    public class ScimResourceReference
    {
        public ScimResourceReference(IScimResourceTypeDefinition resourceDefinition, string resourceId)
        {
            ResourceDefinition = resourceDefinition;
            ResourceId = resourceId;
        }

        public IScimResourceTypeDefinition ResourceDefinition { get; private set; }

        public string ResourceId { get; private set; }
    }
}