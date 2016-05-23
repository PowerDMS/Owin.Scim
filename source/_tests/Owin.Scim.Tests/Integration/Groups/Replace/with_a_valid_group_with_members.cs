using System.Linq;

namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;
    using Model.Users;

    public class with_a_valid_group_with_members : when_replacing_a_group
    {
        Establish context = () =>
        {
            ExistingUser1 = CreateUser(new ScimUser {UserName = Users.UserNameUtility.GenerateUserName()});
            ExistingUser2 = CreateUser(new ScimUser { UserName = Users.UserNameUtility.GenerateUserName() });
            ExistingGroup1 = CreateGroup(new ScimGroup { DisplayName = "existing group 1" });
            ExistingGroup2 = CreateGroup(new ScimGroup
            {
                DisplayName = "existing group 2",
                ExternalId = "hello",
                Members = new[]
                {
                    new Member {Value = ExistingUser1.Id, Type = "user"}
                }
            });

            GroupId = ExistingGroup2.Id;

            GroupDto = new ScimGroup
            {
                Id = GroupId,
                DisplayName = "updated group 2",
                Members = new[]
                {
                    new Member {Value = ExistingUser2.Id, Type = "user"},
                    new Member {Value = ExistingGroup1.Id, Type = "group"} 
                }
            };
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_contain_id = () => CreatedGroup.Id.ShouldEqual(ExistingGroup2.Id);

        It should_contain_meta = () =>
        {
            CreatedGroup.Meta.ShouldNotBeNull();
            CreatedGroup.Meta.Created.ShouldEqual(ExistingGroup2.Meta.Created);
            CreatedGroup.Meta.LastModified.ShouldBeGreaterThan(ExistingGroup2.Meta.LastModified);

            CreatedGroup.Meta.Location.ShouldNotBeNull();
            CreatedGroup.Meta.Location.ShouldEqual(Response.Headers.Location);
            CreatedGroup.Meta.Location.ShouldEqual(ExistingGroup2.Meta.Location);

            CreatedGroup.Meta.Version.ShouldNotBeNull();
            CreatedGroup.Meta.Version.ShouldEqual(Response.Headers.ETag.ToString());
            CreatedGroup.Meta.Version.ShouldNotEqual(ExistingGroup2.Meta.Version);
        };

        It should_contain_new_attribute_value = () => CreatedGroup.DisplayName.ShouldEqual(GroupDto.DisplayName);

        It should_clear_external_id = () => CreatedGroup.ExternalId.ShouldBeNull();

        It should_contain_two_members = () => CreatedGroup.Members.Count().ShouldEqual(2);

        private static ScimUser ExistingUser1;
        private static ScimUser ExistingUser2;
        private static ScimGroup ExistingGroup1;
        private static ScimGroup ExistingGroup2;
    }
}