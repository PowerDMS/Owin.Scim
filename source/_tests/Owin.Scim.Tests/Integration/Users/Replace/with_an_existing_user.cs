namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class with_an_existing_user : when_replacing_a_user
    {
        Establish context = async () =>
        {
            var existingUser = new User
            {
                UserName = "daniel"
            };

            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(existingUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            UserId = (await userRecord.Content.ReadAsAsync<User>()).Id;

            UserDto = new User
            {
                Id = UserId,
                UserName = "daniel",
                PreferredLanguage = "en-US"
            };
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_contain_the_new_values = async () => (await Response.Content.ReadAsAsync<User>()).PreferredLanguage.ShouldEqual("en-US");
    }
}