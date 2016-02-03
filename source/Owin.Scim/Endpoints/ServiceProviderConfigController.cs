namespace Owin.Scim.Endpoints
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Model;

    public class ServiceProviderConfigController : ApiController
    {
        private readonly ScimServerConfiguration _Configuration;

        public ServiceProviderConfigController(ScimServerConfiguration configuration)
        {
            _Configuration = configuration;
        }

        [Route("serviceproviderconfig", Name = "ServiceProviderConfig")]
        public async Task<HttpResponseMessage> Get()
        {
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new ServiceProviderConfig(
                    _Configuration.AuthenticationSchemes));
        }
    }
}