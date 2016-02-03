namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_user : when_creating_a_user
    {
        Establish context = () =>
        {
            UserDto = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_return_the_user = () => Response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).Result.Id.ShouldNotBeEmpty();
    }
}