namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    public class when_creating_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PostAsync("users", new ScimObjectContent<User>(UserDto))
                .AwaitResponse()
                .AsTask;

            CreatedUser = Response.StatusCode == HttpStatusCode.Created
                ? await Response.Content.ScimReadAsAsync<User>().AwaitResponse().AsTask
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? await Response.Content.ScimReadAsAsync<Model.ScimError>().AwaitResponse().AsTask
                : null;
        };
        
        protected static User UserDto;

        protected static User CreatedUser;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}