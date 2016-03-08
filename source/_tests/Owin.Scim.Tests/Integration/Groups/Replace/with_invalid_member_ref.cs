using System.Linq;

namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;
    using Model.Users;

    public class with_invalid_member_ref : when_replacing_a_group
    {
        Establish context = () =>
        {
            ExistingUser1 = CreateUser(new User {UserName = Users.UserNameUtility.GenerateUserName()});
            ExistingUser2 = CreateUser(new User { UserName = Users.UserNameUtility.GenerateUserName() });
            ExistingGroup1 = CreateGroup(new Group { DisplayName = "existing group 1" });
            ExistingGroup2 = CreateGroup(new Group
            {
                DisplayName = "existing group 2",
                ExternalId = "hello",
                Members = new[]
                {
                    new Member {Value = ExistingUser1.Id, Type = "user"},
                    new Member {Value = ExistingGroup1.Id, Type = "group"}
                }
            });

            GroupId = ExistingGroup2.Id;

            GroupDto = new Group
            {
                Id = GroupId,
                DisplayName = "updated group 2",
                Members = new[]
                {
                    new Member {Value = ExistingUser2.Id, Type = "user", Ref = "\\badthing"}
                }
            };
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_syntax = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.InvalidSyntax);

        It should_return_indicate_invalid_attribute = () => Error.Detail.ShouldContain("member.$ref");

        private static User ExistingUser1;
        private static User ExistingUser2;
        private static Group ExistingGroup1;
        private static Group ExistingGroup2;
    }
}