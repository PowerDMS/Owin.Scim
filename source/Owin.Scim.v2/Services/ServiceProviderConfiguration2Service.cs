namespace Owin.Scim.v2.Services
{
    using Configuration;

    using Model;

    using Scim.Model;
    using Scim.Services;

    public class ServiceProviderConfiguration2Service : ServiceProviderConfigurationServiceBase
    {
        public ServiceProviderConfiguration2Service(
            ScimServerConfiguration serverConfiguration, 
            IResourceVersionProvider versionProvider) 
            : base(serverConfiguration, versionProvider)
        {
        }

        protected override ServiceProviderConfiguration CreateServiceProviderConfiguration()
        {
            return new ServiceProviderConfiguration2(
                ServerConfiguration.GetFeature(ScimFeatureType.Patch),
                ServerConfiguration.GetFeature<ScimFeatureBulk>(ScimFeatureType.Bulk),
                ServerConfiguration.GetFeature<ScimFeatureFilter>(ScimFeatureType.Filter),
                ServerConfiguration.GetFeature(ScimFeatureType.ChangePassword),
                ServerConfiguration.GetFeature(ScimFeatureType.Sort),
                ServerConfiguration.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag),
                ServerConfiguration.AuthenticationSchemes);
        }
    }
}