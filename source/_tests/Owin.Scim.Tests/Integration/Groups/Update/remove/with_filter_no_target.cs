namespace Owin.Scim.Tests.Integration.Groups.Update.remove
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class with_filter_no_target : when_updating_a_group
    {
        Establish context = () =>
        {
            ExistingUser = CreateUser(new ScimUser {UserName = Users.UserNameUtility.GenerateUserName()});
            var anotherUser = CreateUser(new ScimUser {UserName = Users.UserNameUtility.GenerateUserName()});
            GroupToUpdate = CreateGroup(
                new ScimGroup
                {
                    DisplayName = "groupToUpdate",
                    ExternalId = "externalId",
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
                            ""op"":""remove"",
                            ""path"": ""members[type eq \""group\""]""
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

        It should_not_change_resource = () => UpdatedGroup.ShouldBeLike(GroupToUpdate);

        private static ScimGroup GroupToUpdate;
        private static ScimUser ExistingUser;
    }
}