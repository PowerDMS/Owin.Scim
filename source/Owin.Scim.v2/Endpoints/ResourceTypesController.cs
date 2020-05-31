﻿namespace Owin.Scim.v2.Endpoints
{
    using System.Net.Http;
    using System.Threading.Tasks;

    using Configuration;

    using Extensions;
    using Owin.Scim.Querying;
    using Owin.Scim.v2.Model;
    using Scim.Endpoints;

    using Services;

    [AllowAnonymous]
    [RoutePrefix(ScimConstantsV2.Endpoints.ResourceTypes)]
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
            //            if (AmbientRequestService.QueryOptions.Filter != null)
            //                return Task.FromResult(Request.CreateResponse(
            //                    HttpStatusCode.Forbidden,
            //                    new ScimError(HttpStatusCode.Forbidden)));

            if (string.IsNullOrWhiteSpace(name))
            {
                return (await _ResourceTypeService.GetResourceTypes())
                    .Let(resourceTypes => SetMetaLocations(resourceTypes, "GetResourceTypes", resourceType => new { name = resourceType.Name }))
                     .Bind(
                        resourceTypes =>
                            new ScimDataResponse<ScimListResponse>(
                                new ScimListResponse2(resourceTypes)
                                )
                    )
                    .ToHttpResponseMessage(Request);
            }
            
            return (await _ResourceTypeService.GetResourceType(name))
                .Let(resourceType => SetMetaLocation(resourceType, "GetResourceTypes", new { name = resourceType.Name }))
                .ToHttpResponseMessage(Request, (resourceType, response) => SetContentLocationHeader(response, "GetResourceTypes", new { name = resourceType.Name }));
        }
    }
}