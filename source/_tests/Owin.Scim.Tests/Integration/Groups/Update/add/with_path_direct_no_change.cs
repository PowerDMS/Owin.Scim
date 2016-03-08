namespace Owin.Scim.Tests.Integration.Groups.Update.add
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class with_path_direct_no_change : when_updating_a_group
    {
        Establish context = () =>
        {
            ExistingUser = CreateUser(new User {UserName = Users.UserNameUtility.GenerateUserName()});
            var anotherUser = CreateUser(new User {UserName = Users.UserNameUtility.GenerateUserName()});
            GroupToUpdate = CreateGroup(
                new Group
                {
                    DisplayName = "groupToUpdate",
                    Members = new []
                    {
                        new Member {Value = anotherUser.Id, Type = "user"}
                    }
                });

            PatchGroupId = GroupToUpdate.Id;

            var stringFormat =
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"":""add"",
                            ""path"": ""externalId"",
                            ""value"": null
                        }]
                    }";

            PatchContent = new StringContent(
                stringFormat.Replace("{0}", ExistingUser.Id),
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_update_version = () => UpdatedGroup.Meta.Version.ShouldEqual(GroupToUpdate.Meta.Version);

        It should_not_update_last_modified = () => UpdatedGroup.Meta.LastModified.ShouldEqual(GroupToUpdate.Meta.LastModified);

        It should_look_identical = () => UpdatedGroup.ShouldBeLike(GroupToUpdate);

        private static Group GroupToUpdate;
        private static User ExistingUser;
    }
}