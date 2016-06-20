namespace Owin.Scim.v1.Services
{
    using Configuration;

    using Model;

    using Scim.Model;
    using Scim.Services;

    public class ServiceProviderConfiguration1Service : ServiceProviderConfigurationServiceBase
    {
        public ServiceProviderConfiguration1Service(
            ScimServerConfiguration serverConfiguration, 
            IResourceVersionProvider versionProvider) 
            : base(serverConfiguration, versionProvider)
        {
        }

        protected override ServiceProviderConfiguration CreateServiceProviderConfiguration()
        {
            return new ServiceProviderConfiguration1(
                new ScimFeature(false), 
                ScimFeatureBulk.CreateUnsupported(),
                ServerConfiguration.GetFeature<ScimFeatureFilter>(ScimFeatureType.Filter),
                ServerConfiguration.GetFeature(ScimFeatureType.ChangePassword),
                ServerConfiguration.GetFeature(ScimFeatureType.Sort),
                ServerConfiguration.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag),
                ServerConfiguration.AuthenticationSchemes);
        }
    }
}