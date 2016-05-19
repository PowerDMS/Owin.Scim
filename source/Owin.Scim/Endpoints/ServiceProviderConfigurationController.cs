namespace Owin.Scim.Endpoints
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Services;

    [AllowAnonymous]
    [RoutePrefix(ScimConstants.Endpoints.ServiceProviderConfig)]
    public class ServiceProviderConfigurationController : ScimControllerBase
    {
        public const string RetrieveServiceProviderConfigurationRouteName = @"GetServiceProviderConfiguration";

        private readonly IServiceProviderConfigurationService _ServiceProviderConfigurationService;

        public ServiceProviderConfigurationController(
            ScimServerConfiguration serverConfiguration,
            IServiceProviderConfigurationService serviceProviderConfigurationService)
            : base(serverConfiguration)
        {
            _ServiceProviderConfigurationService = serviceProviderConfigurationService;
        }

        [Route(Name = RetrieveServiceProviderConfigurationRouteName)]
        public async Task<HttpResponseMessage> Get()
        {
            return (await _ServiceProviderConfigurationService.GetServiceProviderConfiguration())
                .Let(serviceProviderConfig => SetMetaLocation(serviceProviderConfig, RetrieveServiceProviderConfigurationRouteName))
                .ToHttpResponseMessage(Request, (config, response) => SetContentLocationHeader(response, RetrieveServiceProviderConfigurationRouteName));
        }
    }
}