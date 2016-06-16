namespace Owin.Scim.Tests.Integration.Users.Retrieve
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_an_existing_userid : when_retrieving_a_user
    {
        Establish context = async () =>
        {
            var existingUser = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            // Insert the first user so there's one already in-memory.
            Response = await Server
                .HttpClient
                .PostAsync("v2/users", new ObjectContent<ScimUser>(existingUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;
            
            UserId = (await Response.Content.ReadAsAsync<ScimUser2>()).Id;
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
    }
}