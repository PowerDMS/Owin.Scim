using System.Linq;

namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_fully_populated_user : when_creating_a_user
    {
        Establish context = () =>
        {
            var userName = UserNameUtility.GenerateUserName();
            UserDto = new User
            {
                UserName = userName,
                ExternalId = "Id-" + userName
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_return_header_location = () => Response.Headers.Location.ShouldNotBeNull();

        It should_return_the_user = () => CreatedUser.Id.ShouldNotBeEmpty();

        private It should_return_meta = () =>
        {
            CreatedUser.Meta.ShouldNotBeNull();
            CreatedUser.Meta.ResourceType.ShouldEqual("User");
            CreatedUser.Meta.Created.ShouldNotBeNull();
            CreatedUser.Meta.LastModified.ShouldEqual(CreatedUser.Meta.Created);
            CreatedUser.Meta.Location.ShouldNotBeNull();
        };

        It should_echo_create_values = () =>
        {
            CreatedUser.Schemas.ShouldEqual(UserDto.Schemas);
            CreatedUser.UserName.ShouldEqual(UserDto.UserName);
        };
    }
}