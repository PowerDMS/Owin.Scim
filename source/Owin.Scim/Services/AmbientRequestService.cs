namespace Owin.Scim.Services
{
    using System;
    using System.Net.Http;
    using System.Runtime.Remoting.Messaging;
    using System.Text.RegularExpressions;

    using Configuration;

    using Extensions;

    using Microsoft.Owin;

    using Model;

    using Querying;

    public static class AmbientRequestService
    {
        private static readonly Regex _VersionRegex = 
            new Regex("(?:\\/)(v[0-9]+)(?:\\/)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string BasePath
        {
            get { return (string)CallContext.LogicalGetData(ScimConstants.Hosting.BasePath); }
        }

        public static Uri BaseUri
        {
            get { return (Uri)CallContext.LogicalGetData(ScimConstants.Hosting.BaseUri); }
        }

        public static HttpMethod HttpMethod
        {
            get { return (HttpMethod) CallContext.LogicalGetData(ScimConstants.Hosting.HttpMethod); }
        }

        public static ScimQueryOptions QueryOptions
        {
            get { return (ScimQueryOptions) CallContext.LogicalGetData(ScimConstants.Hosting.QueryOptions); }
        }

        public static ScimVersion ProtocolVersion
        {
            get { return (ScimVersion) CallContext.LogicalGetData(ScimConstants.Hosting.Version); }
        }

        internal static void SetRequestInformation(IOwinContext context, ScimServerConfiguration serverConfiguration)
        {
            var request = context.Request;
            var host = request.Scheme + "://" + request.Host.Value;
            var baseUri = new Uri(host + request.PathBase.Value);
            context.Environment[ScimConstants.Hosting.BaseUri] = baseUri;

            CallContext.LogicalSetData(ScimConstants.Hosting.BaseUri, baseUri);
            CallContext.LogicalSetData(ScimConstants.Hosting.BasePath, request.PathBase.Value);
            CallContext.LogicalSetData(ScimConstants.Hosting.Version, GetScimVersion(request, serverConfiguration));
            CallContext.LogicalSetData(ScimConstants.Hosting.HttpMethod, new HttpMethod(context.Request.Method));
            CallContext.LogicalSetData(ScimConstants.Hosting.QueryOptions, context.Request.Query.GetScimQueryOptions(serverConfiguration));
        }

        private static ScimVersion GetScimVersion(IOwinRequest request, ScimServerConfiguration serverConfiguration)
        {
            var result = _VersionRegex.Match(request.Path.ToString());
            if (result.Success)
                return new ScimVersion(result.Groups[1].Value); // e.g. groups[] -> /v0/, v0

            return serverConfiguration.DefaultScimVersion;
        }
    }
}