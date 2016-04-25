namespace Owin.Scim.Endpoints
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Model;

    [RoutePrefix(ScimConstants.Endpoints.ServiceProviderConfig)]
    public class ServiceProviderConfigurationController : ScimControllerBase
    {
        public const string RetrieveServiceProviderConfigurationRouteName = @"GetServiceProviderConfiguration";

        public ServiceProviderConfigurationController(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
        }

        [Route(Name = RetrieveServiceProviderConfigurationRouteName)]
        public async Task<HttpResponseMessage> Get()
        {
            var serviceProviderConfig = (ServiceProviderConfiguration) ServerConfiguration;

            SetMetaLocation(serviceProviderConfig, RetrieveServiceProviderConfigurationRouteName);

            var response = Request.CreateResponse(
                HttpStatusCode.OK, 
                serviceProviderConfig);

            SetContentLocationHeader(response, RetrieveServiceProviderConfigurationRouteName);

            return response;
        }
    }
}