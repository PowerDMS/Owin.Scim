namespace Owin.Scim.Services
{
    using System;
    using System.Threading.Tasks;

    using Configuration;

    using Model;

    public class ServiceProviderConfigurationService : ServiceBase, IServiceProviderConfigurationService
    {
        private readonly Lazy<ServiceProviderConfiguration> _ServiceProviderConfigurationFactory;

        public ServiceProviderConfigurationService(
            ScimServerConfiguration serverConfiguration, 
            IResourceVersionProvider versionProvider) 
            : base(serverConfiguration, versionProvider)
        {
            _ServiceProviderConfigurationFactory = new Lazy<ServiceProviderConfiguration>(
                () =>
                {
                    var config = new ServiceProviderConfiguration(
                        ServerConfiguration.GetFeature(ScimFeatureType.Patch),
                        ServerConfiguration.GetFeature<ScimFeatureBulk>(ScimFeatureType.Bulk),
                        ServerConfiguration.GetFeature<ScimFeatureFilter>(ScimFeatureType.Filter),
                        ServerConfiguration.GetFeature(ScimFeatureType.ChangePassword),
                        ServerConfiguration.GetFeature(ScimFeatureType.Sort),
                        ServerConfiguration.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag),
                        ServerConfiguration.AuthenticationSchemes);

                    SetResourceVersion(config);

                    return config;
                });
        }

        public Task<IScimResponse<ServiceProviderConfiguration>> GetServiceProviderConfiguration()
        {
            return Task.FromResult<IScimResponse<ServiceProviderConfiguration>>(
                new ScimDataResponse<ServiceProviderConfiguration>(
                    _ServiceProviderConfigurationFactory.Value));
        }
    }
}