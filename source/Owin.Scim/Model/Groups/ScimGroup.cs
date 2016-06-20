namespace Owin.Scim.Model.Groups
{
    using System.Collections.Generic;

    using Extensions;

    using Newtonsoft.Json;
    
    public abstract class ScimGroup : Resource
    {
        protected ScimGroup()
        {
            /* 3.3.1.Resource Types
             * When adding a resource to a specific endpoint, the meta attribute
             * "resourceType" SHALL be set by the HTTP service provider to the
             * corresponding resource type for the endpoint.  For example, a POST to
             * the endpoint "/Users" will set "resourceType" to "User", and
             * "/Groups" will set "resourceType" to "Group".
             */
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.Group);
        }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("members")]
        public IEnumerable<Member> Members { get; set; }

        public override int CalculateVersion()
        {
            return new
            {
                Base = base.CalculateVersion(),
                DisplayName,
                Members =  Members.GetMultiValuedAttributeCollectionVersion()
            }.GetHashCode();
        }
    }
}