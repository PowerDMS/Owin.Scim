namespace Owin.Scim.Services
{
    using System.Net.Http;
    using System.Runtime.Remoting.Messaging;

    using Configuration;

    using Extensions;

    using Microsoft.Owin;

    using Querying;

    public static class AmbientRequestMessageService
    {
        public static HttpMethod HttpMethod
        {
            get { return (HttpMethod) CallContext.LogicalGetData("httpMethod"); }
        }

        public static ScimQueryOptions QueryOptions
        {
            get { return (ScimQueryOptions) CallContext.LogicalGetData("queryOptions"); }
        }

        internal static void SetRequestInformation(IOwinContext context, ScimServerConfiguration serverConfiguration)
        {
            CallContext.LogicalSetData("httpMethod", new HttpMethod(context.Request.Method));
            CallContext.LogicalSetData("queryOptions", context.Request.Query.GetScimQueryOptions(serverConfiguration));
        }
    }
}