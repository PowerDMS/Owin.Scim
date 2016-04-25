namespace Owin.Scim.Endpoints
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Extensions;
    
    using Services;

    [RoutePrefix(ScimConstants.Endpoints.ResourceTypes)]
    public class ResourceTypesController : ScimControllerBase
    {
        private readonly IResourceTypeService _ResourceTypeService;

        public ResourceTypesController(ScimServerConfiguration serverConfiguration, IResourceTypeService resourceTypeService) 
            : base(serverConfiguration)
        {
            _ResourceTypeService = resourceTypeService;
        }

        [Route("{name?}", Name = "GetResourceTypes")]
        public async Task<HttpResponseMessage> Get(string name = null)
        {
            // TODO: (DG) uncomment when filters are supported.
//            if (AmbientRequestMessageService.QueryOptions.Filter != null)
//                return Task.FromResult(Request.CreateResponse(
//                    HttpStatusCode.Forbidden,
//                    new ScimError(HttpStatusCode.Forbidden)));

            if (string.IsNullOrWhiteSpace(name))
                return (await _ResourceTypeService.GetResourceTypes())
                    .Let(resourceTypes => SetMetaLocations(resourceTypes, "GetResourceTypes", resourceType => new { name = resourceType.Name }))
                    .ToHttpResponseMessage(Request);
            
            return (await _ResourceTypeService.GetResourceType(name))
                .Let(resourceType => SetMetaLocation(resourceType, "GetResourceTypes", new { name = resourceType.Name }))
                .ToHttpResponseMessage(Request, (resourceType, response) => SetContentLocationHeader(response, "GetResourceTypes", new { name = resourceType.Name }));
        }
    }
}