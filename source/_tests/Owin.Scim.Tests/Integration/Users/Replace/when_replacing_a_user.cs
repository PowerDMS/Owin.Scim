namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Extensions;

    using Machine.Specifications;

    using Model.Users;

    public class when_replacing_a_user : using_a_scim_server
    {
        Because of = async () =>
        {
            Task.Delay(100).Await();

            Response = await Server
                .HttpClient
                .PutAsync("users/" + UserDto.Id, new ScimObjectContent<ScimUser>(UserDto))
                .AwaitResponse()
                .AsTask;

            if (Response.StatusCode == HttpStatusCode.OK)
                await Response.DeserializeTo(() => UpdatedUserRecord); // capture updated user record
        };

        protected static ScimUser UserDto;

        protected static HttpResponseMessage Response;

        protected static ScimUser UpdatedUserRecord;
    }
}