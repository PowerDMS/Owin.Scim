namespace Owin.Scim.Tests.Integration.Groups.Update.remove
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    public class with_no_path : when_updating_a_group
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
                            ""op"":""remove"",
                            ""value"": {
                                ""externalId"": ""updatedExternal"",
                                ""members"":[{
                                    ""value"": ""{0}"",
                                    ""type"": ""user""
                                }],
                            }
                        }]
                    }";

            PatchContent = new StringContent(
                stringFormat.Replace("{0}", ExistingUser.Id),
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_no_target = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.NoTarget);

        private static Group GroupToUpdate;
        private static User ExistingUser;
    }
}