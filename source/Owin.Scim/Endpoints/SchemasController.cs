namespace Owin.Scim.Endpoints
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;

    using Services;

    [RoutePrefix(ScimConstants.Endpoints.Schemas)]
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

        [Route("{schemaId?}", Name = "GetSchemas")]
        public async Task<HttpResponseMessage> GetSchemas(string schemaId = null)
        {
            if (string.IsNullOrWhiteSpace(schemaId))
                return (await _SchemaService.GetSchemas())
                    .Let(schemata => SetMetaLocations(schemata, "GetSchemas", schema => new { schemaId = schema.Id }))
                    .ToHttpResponseMessage(Request);

            return (await _SchemaService.GetSchema(schemaId))
                .Let(schema => SetMetaLocation(schema, "GetSchemas", new { schemaId = schema.Id }))
                .ToHttpResponseMessage(Request, (schema, response) => SetContentLocationHeader(response, "GetSchemas", new { schemaId = schema.Id }));
        }
    }
}