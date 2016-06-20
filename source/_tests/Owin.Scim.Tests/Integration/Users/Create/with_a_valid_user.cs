using System.Linq;

namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System;
    using System.Net;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_a_valid_user : when_creating_a_user
    {
        Establish context = () =>
        {
            CurrentUtcDateTime = DateTime.UtcNow;
            UserDto = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                Password = "Hiworld!1"
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
            CreatedUser.Meta.Version.ShouldEqual(Response.Headers.ETag.ToString());
        };

        It should_echo_create_values = () =>
        {
            CreatedUser.Schemas.ShouldEqual(UserDto.Schemas);
            CreatedUser.UserName.ShouldEqual(UserDto.UserName);
        };

        private static DateTime CurrentUtcDateTime;
    }
}