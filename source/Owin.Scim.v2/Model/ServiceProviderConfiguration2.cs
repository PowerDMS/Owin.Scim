namespace Owin.Scim.v2.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Scim.Model;

    public class ServiceProviderConfiguration2 : ServiceProviderConfiguration
    {
        public ServiceProviderConfiguration2(
            ScimFeature featurePatch, 
            ScimFeatureBulk featureBulk, 
            ScimFeatureFilter featureFilter, 
            ScimFeature featureChangePassword, 
            ScimFeature featureSort, 
            ScimFeatureETag featureETag, 
            IEnumerable<AuthenticationScheme> authenticationSchemes = null)
        {
            Bulk = featureBulk ?? ScimFeatureBulk.CreateUnsupported();
            Filter = featureFilter ?? ScimFeatureFilter.CreateUnsupported();
            Patch = featurePatch;
            ChangePassword = featureChangePassword;
            Sort = featureSort;
            ETag = featureETag;
            AuthenticationSchemes = authenticationSchemes == null
                ? new List<AuthenticationScheme>()
                : authenticationSchemes.ToList();
        }

        [JsonConstructor]
        private ServiceProviderConfiguration2() { }

        public override string SchemaIdentifier
        {
            get { return ScimConstantsV2.Schemas.ServiceProviderConfig; }
        }
    }
}