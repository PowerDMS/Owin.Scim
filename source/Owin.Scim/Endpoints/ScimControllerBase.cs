namespace Owin.Scim.Endpoints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;

    using Configuration;

    using Model;

    public class ScimControllerBase : ApiController
    {
        public ScimControllerBase(ScimServerConfiguration serverConfiguration)
        {
            ServerConfiguration = serverConfiguration;
        }

        protected ScimServerConfiguration ServerConfiguration { get; private set; }

        [NonAction]
        protected void SetETagHeader<T>(HttpResponseMessage response, T resource) 
            where T : Resource
        {
            if (!string.IsNullOrWhiteSpace(resource.Meta.Version) && ServerConfiguration.IsFeatureSupported(ScimFeatureType.ETag))
                response.Headers.ETag = EntityTagHeaderValue.Parse(resource.Meta.Version);
        }

        [NonAction]
        protected void SetContentLocationHeader(HttpResponseMessage response, string routeName, object routeValues = null)
        {
            response.Headers.Location = new Uri(Request.GetUrlHelper().Link(routeName, routeValues));
        }

        [NonAction]
        protected IEnumerable<T> SetMetaLocations<T>(IEnumerable<T> items, string routeName, Func<T, object> routeValueFactory = null) 
            where T : Resource
        {
            var urlHelper = Request.GetUrlHelper();
            foreach (var item in items.Where(item => item.Meta != null))
            {
                var routeValues = routeValueFactory == null ? null : routeValueFactory(item);
                item.Meta.Location = new Uri(urlHelper.Link(routeName, routeValues));
            }

            return items;
        }

        [NonAction]
        protected T SetMetaLocation<T>(T item, string routeName, object routeValues = null) 
            where T : Resource
        {
            if (item.Meta != null)
                item.Meta.Location = new Uri(Request.GetUrlHelper().Link(routeName, routeValues));

            return item;
        }

        [NonAction]
        protected Uri GetGroupUri(string routeName, string groupId)
        {
            var test = new Uri(
                Request
                    .GetUrlHelper()
                    .Link(routeName, new { groupId = groupId }));
            return test;
        }

        [NonAction]
        protected Uri GetUserUri(string routeName, string userId)
        {
            var test = new Uri(
                Request
                    .GetUrlHelper()
                    .Link(routeName, new { userId = userId }));
            return test;
        }
    }
}