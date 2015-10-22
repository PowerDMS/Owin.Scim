namespace Owin.Scim.Tests.Integration.Users.Retrieve
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class with_an_existing_userid : when_retrieving_a_user
    {
        Establish context = async () =>
        {
            var existingUser = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            // Insert the first user so there's one already in-memory.
            Response = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(existingUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;
            
            UserId = (await Response.Content.ReadAsAsync<User>()).Id;
        };

        It should_return_ok = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);
    }
}