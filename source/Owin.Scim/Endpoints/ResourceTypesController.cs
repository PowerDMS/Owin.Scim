namespace Owin.Scim.Endpoints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Configuration;

    using Model;

    public class ResourceTypesController : ScimControllerBase
    {
        public ResourceTypesController(ScimServerConfiguration scimServerConfiguration) 
            : base(scimServerConfiguration)
        {
        }

        [Route("resourcetypes/{name?}", Name = "GetResourceTypes")]
        public Task<HttpResponseMessage> Get(string name = null)
        {
            HttpResponseMessage response;
            if (string.IsNullOrWhiteSpace(name))
            {
                response = Request.CreateResponse(
                    HttpStatusCode.OK,
                    SetMetaLocations(ScimServerConfiguration.ResourceTypes));
            }
            else
            {
                var resourceType = ScimServerConfiguration.ResourceTypes
                    .SingleOrDefault(rt => rt.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

                response = resourceType == null 
                    ? Request.CreateResponse(
                        HttpStatusCode.NotFound, 
                        new ScimError(
                            HttpStatusCode.NotFound, 
                            detail: string.Format("ResourceType '{0}' does not exist.", name))) 
                    : Request.CreateResponse(HttpStatusCode.OK, SetMetaLocation(resourceType));
            }

            return Task.FromResult(response);
        }

        private IEnumerable<ResourceType> SetMetaLocations(IEnumerable<ResourceType> resourceTypes)
        {
            foreach (var resourceType in resourceTypes)
            {
                SetMetaLocation(resourceType);
            }

            return resourceTypes;
        }

        private ResourceType SetMetaLocation(ResourceType resourceType)
        {
            resourceType.Meta.Location =
                    new Uri(
                        Request
                            .GetUrlHelper()
                            .Link("GetResourceTypes", new { name = resourceType.Name })
                        );

            return resourceType;
        }
    }
}