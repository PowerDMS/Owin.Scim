namespace Owin.Scim.v1.Endpoints
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Scim.Endpoints;
    using Scim.Services;

    [AllowAnonymous]
    [RoutePrefix(ScimConstantsV1.Endpoints.Schemas)]
    public class SchemasController : ScimControllerBase
    {
        private readonly ISchemaService _SchemaService;

        public SchemasController(
            ScimServerConfiguration serverConfiguration,
            ISchemaService schemaService) 
            : base(serverConfiguration)
        {
            _SchemaService = schemaService;
        }

        [Route]
        public async Task<HttpResponseMessage> GetSchemas()
        {
            return (await _SchemaService.GetSchemas())
                .ToHttpResponseMessage(Request);
        }
    }
}