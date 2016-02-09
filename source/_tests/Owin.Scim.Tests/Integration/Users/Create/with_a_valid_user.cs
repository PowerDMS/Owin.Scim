using System.Linq;

namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_valid_user : when_creating_a_user
    {
        Establish context = () =>
        {
            // TODO: CY - still worry that server is emitting dates in the server's timezone
            CurrentUtcDateTime = DateTime.Now;

            UserDto = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_return_header_location = () => Response.Headers.Location.ShouldNotBeNull();

        It should_return_the_user = () => CreatedUser.Id.ShouldNotBeEmpty();

        It should_return_meta = () =>
        {
            CreatedUser.Meta.ShouldNotBeNull();
            CreatedUser.Meta.ResourceType.ShouldEqual("User");
            CreatedUser.Meta.Created.ShouldNotBeNull();
            CreatedUser.Meta.Created.ShouldBeGreaterThanOrEqualTo(CurrentUtcDateTime);
            CreatedUser.Meta.LastModified.ShouldEqual(CreatedUser.Meta.Created);
            CreatedUser.Meta.Location.ShouldNotBeNull();
        };

        It should_echo_create_values = () =>
        {
            CreatedUser.Schemas.ShouldEqual(UserDto.Schemas);
            CreatedUser.UserName.ShouldEqual(UserDto.UserName);
        };

        private static DateTime CurrentUtcDateTime;
    }
}