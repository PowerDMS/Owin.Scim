namespace Owin.Scim.Tests.Integration.Users.Query
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Scim.Querying;

    public class when_querying_users : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .GetAsync("users/?" + QueryString)
                .AwaitResponse()
                .AsTask;
            
            ListResponse = Response.StatusCode == HttpStatusCode.OK
                ? await Response.Content.ScimReadAsAsync<ScimListResponse>().AwaitResponse().AsTask
                : null;
        };

        protected static string QueryString;

        protected static HttpResponseMessage Response;

        protected static ScimListResponse ListResponse;
    }
}