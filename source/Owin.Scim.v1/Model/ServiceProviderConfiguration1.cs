namespace Owin.Scim.v1.Model
{
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Scim.Model;

    public class ServiceProviderConfiguration1 : ServiceProviderConfiguration
    {
        public ServiceProviderConfiguration1(
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
        private ServiceProviderConfiguration1() { }

        public override string SchemaIdentifier
        {
            get { return ScimConstantsV1.Schemas.ServiceProviderConfig; }
        }
    }
}