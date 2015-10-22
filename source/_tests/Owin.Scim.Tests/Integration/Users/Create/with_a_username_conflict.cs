namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_username_conflict : when_creating_a_user
    {
        Establish context = async () =>
        {
            UserDto = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            // Insert the first user so there's one already in-memory.
            Response = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(UserDto, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;
        };

        It should_return_conflict = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Conflict);
    }
}