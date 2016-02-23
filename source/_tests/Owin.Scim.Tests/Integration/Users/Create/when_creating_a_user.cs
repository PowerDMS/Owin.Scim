namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Collections.Generic;
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
                .PostAsync("users", new ObjectContent<User>(UserDto, new ScimJsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            CreatedUser = Response.StatusCode == HttpStatusCode.Created
                ? Response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).Result
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? Response.Content.ReadAsAsync<Model.ScimError>(ScimJsonMediaTypeFormatter.AsArray()).Result
                : null;
        };
        
        protected static User UserDto;

        protected static User CreatedUser;

        protected static HttpResponseMessage Response;

        protected static Model.ScimError Error;
    }
}