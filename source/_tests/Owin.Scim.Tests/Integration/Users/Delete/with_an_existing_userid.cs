namespace Owin.Scim.Tests.Integration.Users.Delete
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class with_an_existing_userid : when_deleting_a_user
    {
        Establish context = async () =>
        {
            var existingUser = new ScimUser
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            // Insert the first user so there's one already in-memory.
            Response = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<ScimUser>(existingUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            UserId = (await Response.Content.ReadAsAsync<ScimUser>()).Id;
        };

        It should_return_no_content = () => Response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
    }
}