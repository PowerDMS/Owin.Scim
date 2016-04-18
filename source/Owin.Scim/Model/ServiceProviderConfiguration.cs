namespace Owin.Scim.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Extensions;

    using Newtonsoft.Json;

    [ScimTypeDefinition(typeof(ServiceProviderConfigurationDefinition))]
    public class ServiceProviderConfiguration : Resource
    {
        private readonly ScimFeature _Patch;

        private readonly ScimFeatureBulk _Bulk;

        private readonly ScimFeatureFilter _Filter;

        private readonly ScimFeature _ChangePassword;

        private readonly ScimFeature _Sort;

        private readonly ScimFeature _ETag;

        private readonly IEnumerable<AuthenticationScheme> _AuthenticationSchemes;

        public ServiceProviderConfiguration(
            ScimFeature featurePatch,
            ScimFeatureBulk featureBulk,
            ScimFeatureFilter featureFilter,
            ScimFeature featureChangePassword,
            ScimFeature featureSort,
            ScimFeature featureETag,
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

            _Bulk = featureBulk ?? ScimFeatureBulk.CreateUnsupported();
            _Filter = featureFilter ?? ScimFeatureFilter.CreateUnsupported();
            _Patch = featurePatch;
            _ChangePassword = featureChangePassword;
            _Sort = featureSort;
            _ETag = featureETag;
            _AuthenticationSchemes = authenticationSchemes?.ToList() ?? new List<AuthenticationScheme>();
        }

        [JsonProperty("documentationUri")]
        public Uri DocumentationUri
        {
            get { return null; }
        }

        [JsonProperty("patch")]
        public ScimFeature Patch { get { return _Patch; } }

        [JsonProperty("bulk")]
        public ScimFeatureBulk Bulk { get { return _Bulk; } }

        [JsonProperty("filter")]
        public ScimFeatureFilter Filter { get { return _Filter; } }

        [JsonProperty("changePassword")]
        public ScimFeature ChangePassword { get { return _ChangePassword; } }

        [JsonProperty("sort")]
        public ScimFeature Sort { get { return _Sort; } }

        [JsonProperty("etag")]
        public ScimFeature ETag { get { return _ETag; } }

        [JsonProperty("authenticationSchemes")]
        public IEnumerable<AuthenticationScheme> AuthenticationSchemes
        {
            get { return _AuthenticationSchemes; }
        }
        
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