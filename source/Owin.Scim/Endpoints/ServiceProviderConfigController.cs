namespace Owin.Scim.Endpoints
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Model;

    public class ServiceProviderConfigController : ControllerBase
    {
        public ServiceProviderConfigController(ScimServerConfiguration serverConfiguration)
            : base(serverConfiguration)
        {
        }

        [Route("serviceproviderconfig", Name = "ServiceProviderConfig")]
        public async Task<HttpResponseMessage> Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, (ServiceProviderConfig)ServerConfiguration);
        }
    }
}