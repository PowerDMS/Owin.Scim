namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_multiple_validation_errors : when_creating_a_user
    {
        Establish context = async () =>
        {
            var userName = UserNameUtility.GenerateUserName();
            var initialUser = new User
            {
                UserName = userName
            };

            UserDto = new User
            {
                UserName = userName,
                PreferredLanguage = "invalidLanguage"
            };

            // Insert the first user so there's one already in-memory.
            Response = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(initialUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_contain_two_errors = () => Response.Content.ReadAsAsync<IEnumerable<ScimError>>(ScimJsonMediaTypeFormatter.AsArray()).Result.Count().ShouldEqual(2);
    }
}