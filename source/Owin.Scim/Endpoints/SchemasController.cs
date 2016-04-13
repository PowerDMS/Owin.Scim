namespace Owin.Scim.Endpoints
{
    using System.Net.Http;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Services;

    public class SchemasController : ScimControllerBase
    {
        private readonly ISchemaService _SchemaService;

        public SchemasController(
            ScimServerConfiguration scimServerConfiguration,
            ISchemaService schemaService) 
            : base(scimServerConfiguration)
        {
            _SchemaService = schemaService;
        }

        [Route("schemas/{schemaId?}", Name = "GetSchemas")]
        public HttpResponseMessage GetSchemas(string schemaId = null)
        {
            if (string.IsNullOrWhiteSpace(schemaId))
                return _SchemaService.GetSchemas()
                    .ToHttpResponseMessage(Request);

            return _SchemaService.GetSchema(schemaId)
                .ToHttpResponseMessage(Request);
        }
    }
}