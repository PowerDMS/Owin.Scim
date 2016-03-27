namespace Owin.Scim.Services
{
    using System.Net.Http;
    using System.Runtime.Remoting.Messaging;

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

        internal static void SetRequestInformation(IOwinContext context)
        {
            // TODO: (DG) Find a better way to get querystring name value pairs that doesn't rely on HttpRequestMessage
            var req = new HttpRequestMessage(new HttpMethod(context.Request.Method), context.Request.Uri);
            CallContext.LogicalSetData("httpMethod", new HttpMethod(context.Request.Method));
            CallContext.LogicalSetData("queryOptions", new ScimQueryOptions(req.GetQueryNameValuePairs()));
        }
    }
}