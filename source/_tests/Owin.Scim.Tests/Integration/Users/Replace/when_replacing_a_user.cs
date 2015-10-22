namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public abstract class when_replacing_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            Response = await Server
                .HttpClient
                .PutAsync("users/" + UserId, new ObjectContent<User>(UserDto, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            var res = (await Response.Content.ReadAsStringAsync());
        };

        protected static User UserDto;

        protected static string UserId;

        protected static HttpResponseMessage Response;
    }
}