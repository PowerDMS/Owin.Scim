namespace Owin.Scim.Model
{
    using System;
    using System.Collections.Generic;

    using Extensions;

    using Newtonsoft.Json;
    
    public abstract class ServiceProviderConfiguration : Resource
    {
        protected ServiceProviderConfiguration()
        {
            /* 3.3.1.Resource Types
             * When adding a resource to a specific endpoint, the meta attribute
             * "resourceType" SHALL be set by the HTTP service provider to the
             * corresponding resource type for the endpoint.  For example, a POST to
             * the endpoint "/Users" will set "resourceType" to "User", and
             * "/Groups" will set "resourceType" to "Group".
             */
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.ServiceProviderConfig);
        }

        [JsonProperty("documentationUri")]
        public Uri DocumentationUri { get; protected set; }

        [JsonProperty("patch")]
        public ScimFeature Patch { get; protected set; }

        [JsonProperty("bulk")]
        public ScimFeatureBulk Bulk { get; protected set; }

        [JsonProperty("filter")]
        public ScimFeatureFilter Filter { get; protected set; }

        [JsonProperty("changePassword")]
        public ScimFeature ChangePassword { get; protected set; }

        [JsonProperty("sort")]
        public ScimFeature Sort { get; protected set; }

        [JsonProperty("etag")]
        public ScimFeatureETag ETag { get; protected set; }

        [JsonProperty("authenticationSchemes")]
        public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; protected set; }
        
        public override int CalculateVersion()
        {
            return new
            {
                DocumentationUri,
                Patch = Patch.GetETagHashCode(),
                Bulk = Bulk.GetETagHashCode(),
                Filter = Filter.GetETagHashCode(),
                ChangePassword = ChangePassword.GetETagHashCode(),
                Sort = Sort.GetETagHashCode(),
                ETag = ETag.GetETagHashCode(),
                AuthenticationSchemes = AuthenticationSchemes.GetMultiValuedAttributeCollectionVersion()
            }.GetHashCode();
        }
    }
}