namespace Owin.Scim.Endpoints
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Model;

    public class ServiceProviderConfigurationController : ScimControllerBase
    {
        public ServiceProviderConfigurationController(ScimServerConfiguration scimServerConfiguration)
            : base(scimServerConfiguration)
        {
        }

        [Route("serviceproviderconfig", Name = "ServiceProviderConfiguration")]
        public async Task<HttpResponseMessage> Get()
        {
            var serviceProviderConfig = (ServiceProviderConfiguration) ScimServerConfiguration;
            var response = Request.CreateResponse(
                HttpStatusCode.OK, 
                serviceProviderConfig);

            SetLocationHeader(response, serviceProviderConfig, "ServiceProviderConfiguration");

            return response;
        }
    }
}