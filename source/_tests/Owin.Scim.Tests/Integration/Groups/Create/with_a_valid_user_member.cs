namespace Owin.Scim.Tests.Integration.Groups.Create
{
    using System;
    using System.Linq;
    using System.Net;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class with_a_valid_user_member : when_creating_a_group
    {
        Establish context = () =>
        {
            TestStartTime = DateTime.UtcNow;

            ExistingUser = CreateUser(new ScimUser { UserName = Users.UserNameUtility.GenerateUserName() });
            ExistingGroup = CreateGroup(new ScimGroup {DisplayName = Users.UserNameUtility.GenerateUserName()});

            GroupDto = new ScimGroup
            {
                DisplayName = "hello",
                ExternalId = "hello",
                Members = new []
                {
                    new Member {Value = ExistingUser.Id, Type = "user"},
                    new Member {Value = ExistingGroup.Id, Type = "group"},
                }
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_contain_id = () => CreatedGroup.Id.ShouldNotBeEmpty();

        It should_contain_meta = () =>
        {
            CreatedGroup.Meta.ShouldNotBeNull();
            CreatedGroup.Meta.Created.ShouldBeGreaterThanOrEqualTo(TestStartTime);
            CreatedGroup.Meta.LastModified.ShouldEqual(CreatedGroup.Meta.Created);
            CreatedGroup.Meta.Location.ShouldNotBeNull();
            CreatedGroup.Meta.Location.ShouldEqual(Response.Headers.Location);
            CreatedGroup.Meta.Version.ShouldNotBeNull();
            CreatedGroup.Meta.Version.ShouldEqual(Response.Headers.ETag.ToString());
        };

        It should_contain_new_member = () => CreatedGroup.Members.First().Value.ShouldEqual(ExistingUser.Id);

        It should_canonize_member_type = () => CreatedGroup.Members.First().Type.ShouldEqual("User");

        private static ScimUser ExistingUser;
        private static ScimGroup ExistingGroup;
        private static DateTime TestStartTime;
    }
}