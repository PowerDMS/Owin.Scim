namespace Owin.Scim.v1.Endpoints
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Scim.Endpoints;
    using Scim.Services;

    using v1;

    [AllowAnonymous]
    [RoutePrefix(ScimConstantsV1.Endpoints.ServiceProviderConfig)]
    public class ServiceProviderConfigurationController : ScimControllerBase
    {
        public const string GetServiceProviderConfigurationRouteName = @"GetServiceProviderConfiguration1";

        private readonly IServiceProviderConfigurationService _ServiceProviderConfigurationService;

        public ServiceProviderConfigurationController(
            ScimServerConfiguration serverConfiguration,
            IServiceProviderConfigurationService serviceProviderConfigurationService)
            : base(serverConfiguration)
        {
            _ServiceProviderConfigurationService = serviceProviderConfigurationService;
        }

        [Route(Name = GetServiceProviderConfigurationRouteName)]
        public async Task<HttpResponseMessage> Get()
        {
            return (await _ServiceProviderConfigurationService.GetServiceProviderConfiguration())
                .Let(serviceProviderConfig => SetMetaLocation(serviceProviderConfig, GetServiceProviderConfigurationRouteName))
                .ToHttpResponseMessage(Request, (config, response) => SetContentLocationHeader(response, GetServiceProviderConfigurationRouteName));
        }
    }
}