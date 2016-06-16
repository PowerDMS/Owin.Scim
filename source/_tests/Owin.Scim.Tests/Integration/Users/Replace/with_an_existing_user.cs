namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_an_existing_user : when_replacing_a_user
    {
        Establish context = async () =>
        {
            var userName = UserNameUtility.GenerateUserName();
            var existingUser = new ScimUser2
            {
                UserName = userName
            };

            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("v2/users", new ObjectContent<ScimUser>(existingUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            var userId = (await userRecord.Content.ReadAsAsync<ScimUser2>()).Id;

            UserDto = new ScimUser2
            {
                Id = userId,
                UserName = userName,
                PreferredLanguage = "en-US"
            };
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_contain_the_new_values = () => UpdatedUserRecord.PreferredLanguage.ShouldEqual("en-US");
    }
}