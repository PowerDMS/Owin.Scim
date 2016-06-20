namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class when_creating_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PostAsync("v2/users", new ScimObjectContent<ScimUser>(UserDto))
                .AwaitResponse()
                .AsTask;

            CreatedUser = Response.StatusCode == HttpStatusCode.Created
                ? await Response.Content.ScimReadAsAsync<ScimUser2>().AwaitResponse().AsTask
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? await Response.Content.ScimReadAsAsync<Model.ScimError>().AwaitResponse().AsTask
                : null;
        };
        
        protected static ScimUser UserDto;

        protected static ScimUser CreatedUser;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}