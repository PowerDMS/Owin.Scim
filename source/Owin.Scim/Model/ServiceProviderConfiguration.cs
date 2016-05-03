namespace Owin.Scim.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;

    using Newtonsoft.Json;
    
    public class ServiceProviderConfiguration : Resource
    {
        public ServiceProviderConfiguration(
            ScimFeature featurePatch,
            ScimFeatureBulk featureBulk,
            ScimFeatureFilter featureFilter,
            ScimFeature featureChangePassword,
            ScimFeature featureSort,
            ScimFeatureETag featureETag,
            IEnumerable<AuthenticationScheme> authenticationSchemes = null)
        {
            /* 3.3.1.Resource Types
             * When adding a resource to a specific endpoint, the meta attribute
             * "resourceType" SHALL be set by the HTTP service provider to the
             * corresponding resource type for the endpoint.  For example, a POST to
             * the endpoint "/Users" will set "resourceType" to "User", and
             * "/Groups" will set "resourceType" to "Group".
             */
            Meta = new ResourceMetadata(ScimConstants.ResourceTypes.ServiceProviderConfig);

            Id = "blahblahblah";
            Bulk = featureBulk ?? ScimFeatureBulk.CreateUnsupported();
            Filter = featureFilter ?? ScimFeatureFilter.CreateUnsupported();
            Patch = featurePatch;
            ChangePassword = featureChangePassword;
            Sort = featureSort;
            ETag = featureETag;
            AuthenticationSchemes = authenticationSchemes?.ToList() ?? new List<AuthenticationScheme>();
        }

        [JsonConstructor]
        private ServiceProviderConfiguration() { }

        [JsonProperty("documentationUri")]
        public Uri DocumentationUri { get; private set; }

        [JsonProperty("patch")]
        public ScimFeature Patch { get; private set; }

        [JsonProperty("bulk")]
        public ScimFeatureBulk Bulk { get; private set; }

        [JsonProperty("filter")]
        public ScimFeatureFilter Filter { get; private set; }

        [JsonProperty("changePassword")]
        public ScimFeature ChangePassword { get; private set; }

        [JsonProperty("sort")]
        public ScimFeature Sort { get; private set; }

        [JsonProperty("etag")]
        public ScimFeatureETag ETag { get; private set; }

        [JsonProperty("authenticationSchemes")]
        public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; private set; }
        
        public override string SchemaIdentifier
        {
            get { return ScimConstants.Schemas.ServiceProviderConfig; }
        }

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