namespace Owin.Scim.Services
{
    using System;
    using System.Net.Http;
    using System.Runtime.Remoting.Messaging;

    using Configuration;

    using Extensions;

    using Microsoft.Owin;

    using Querying;

    public static class AmbientRequestService
    {
        public static string BasePath
        {
            get { return (string)CallContext.LogicalGetData(ScimConstants.Owin.BasePath); }
        }

        public static Uri BaseUri
        {
            get { return (Uri)CallContext.LogicalGetData(ScimConstants.Owin.BaseUri); }
        }

        public static HttpMethod HttpMethod
        {
            get { return (HttpMethod) CallContext.LogicalGetData(ScimConstants.Owin.HttpMethod); }
        }

        public static ScimQueryOptions QueryOptions
        {
            get { return (ScimQueryOptions) CallContext.LogicalGetData(ScimConstants.Owin.QueryOptions); }
        }

        internal static void SetRequestInformation(IOwinContext context, ScimServerConfiguration serverConfiguration)
        {
            var request = context.Request;
            var host = request.Scheme + "://" + request.Host.Value;
            var baseUri = new Uri(host + request.PathBase.Value);
            context.Environment[ScimConstants.Owin.BaseUri] = baseUri;

            CallContext.LogicalSetData(ScimConstants.Owin.BaseUri, baseUri);
            CallContext.LogicalSetData(ScimConstants.Owin.BasePath, request.PathBase.Value);
            CallContext.LogicalSetData(ScimConstants.Owin.HttpMethod, new HttpMethod(context.Request.Method));
            CallContext.LogicalSetData(ScimConstants.Owin.QueryOptions, context.Request.Query.GetScimQueryOptions(serverConfiguration));
        }
    }
}