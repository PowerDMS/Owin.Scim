namespace Owin.Scim.Tests.Integration.Users.Delete
{
    using System.Net.Http;

    using Machine.Specifications;

    public abstract class when_deleting_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .DeleteAsync("users/" + UserId)
                .AwaitResponse()
                .AsTask;
        };

        protected static string UserId;

        protected static HttpResponseMessage Response;
    }
}