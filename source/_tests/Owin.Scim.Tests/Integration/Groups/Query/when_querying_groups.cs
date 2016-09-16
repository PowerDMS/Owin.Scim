using Owin.Scim.v2.Model;

namespace Owin.Scim.Tests.Integration.Groups.Query
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Scim.Querying;

    public class when_querying_groups : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .GetAsync("v2/groups/?" + QueryString)
                .AwaitResponse()
                .AsTask;
            
            ListResponse = Response.StatusCode == HttpStatusCode.OK
                ? await Response.Content.ScimReadAsAsync<ScimListResponse2>().AwaitResponse().AsTask
                : null;
        };

        protected static string QueryString;

        protected static HttpResponseMessage Response;

        protected static ScimListResponse ListResponse;
    }
}