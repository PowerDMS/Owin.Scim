namespace Owin.Scim.Services
{
    using System;
    using System.Threading.Tasks;

    using Configuration;

    using Model;

    public abstract class ServiceProviderConfigurationServiceBase : ServiceBase, IServiceProviderConfigurationService
    {
        private readonly Lazy<ServiceProviderConfiguration> _ServiceProviderConfigurationFactory;

        protected ServiceProviderConfigurationServiceBase(
            ScimServerConfiguration serverConfiguration, 
            IResourceVersionProvider versionProvider) 
            : base(serverConfiguration, versionProvider)
        {
            _ServiceProviderConfigurationFactory = new Lazy<ServiceProviderConfiguration>(
                () =>
                {
                    var config = CreateServiceProviderConfiguration();

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

        protected abstract ServiceProviderConfiguration CreateServiceProviderConfiguration();
    }
}