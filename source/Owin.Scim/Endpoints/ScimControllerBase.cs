namespace Owin.Scim.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;

    using Configuration;

    using Model;

    public class ScimControllerBase : ApiController
    {
        public ScimControllerBase(ScimServerConfiguration scimServerConfiguration)
        {
            ScimServerConfiguration = scimServerConfiguration;
        }

        protected ScimServerConfiguration ScimServerConfiguration { get; private set; }

        protected void SetLocationHeader(
            HttpResponseMessage response, 
            Resource resource,
            string routeName,
            object routeValues = null)
        {
            var resourceLocation = new Uri(Request
                .GetUrlHelper()
                .Link(routeName, routeValues));

            resource.Meta.Location = resourceLocation;
            response.Headers.Location = resourceLocation;
        }

        protected void SetETagHeader(HttpResponseMessage response, Resource resource)
        {
            if (ScimServerConfiguration.IsFeatureSupported(ScimFeatureType.ETag))
            {
                response.Headers.ETag = EntityTagHeaderValue.Parse(resource.Meta.Version);
            }
        }
    }
}