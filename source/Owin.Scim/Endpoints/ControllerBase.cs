namespace Owin.Scim.Endpoints
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;

    using Configuration;

    using Model;

    public class ControllerBase : ApiController
    {
        public ControllerBase(ScimServerConfiguration serverConfiguration)
        {
            ServerConfiguration = serverConfiguration;
        }

        protected ScimServerConfiguration ServerConfiguration { get; private set; }

        protected void SetLocationHeader(
            HttpResponseMessage response, 
            Resource resource,
            string routeName,
            object routeValues)
        {
            var resourceLocation = new Uri(Request
                .GetUrlHelper()
                .Link(routeName, routeValues));

            resource.Meta.Location = resourceLocation;
            response.Headers.Location = resourceLocation;
        }

        protected void SetETagHeader(HttpResponseMessage response, Resource resource)
        {
            var etagConfig = ServerConfiguration.GetFeature<ScimFeatureETag>(ScimFeatureType.ETag);
            if (etagConfig.Supported)
            {
                response.Headers.ETag = new EntityTagHeaderValue("\"" + resource.Meta.Version + "\"", etagConfig.IsWeak);
            }
        }
    }
}