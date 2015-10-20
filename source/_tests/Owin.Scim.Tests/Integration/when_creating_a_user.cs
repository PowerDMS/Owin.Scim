namespace Owin.Scim.Tests.Integration
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class when_creating_a_user : when_modifying_users
    {
        Establish context = () =>
        {

        };

        Because of = async () =>
        {
            var userDto = new User
            {
                UserName = "daniel"
            };

            Response = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(userDto, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            Message = await Response.Content.ReadAsStringAsync().AwaitResponse().AsTask;
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        private static HttpResponseMessage Response;

        private static String Message;
    }
}