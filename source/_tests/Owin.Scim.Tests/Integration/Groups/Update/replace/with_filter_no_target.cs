namespace Owin.Scim.Tests.Integration.Groups.Update.replace
{
    using System.Linq;
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
                            ""op"":""replace"",
                            ""path"": ""members[type eq \""group\""]"",
                            ""value"":  {
                                ""value"": ""{0}""
                            }
                        }]
                    }";

            PatchContent = new StringContent(
                stringFormat.Replace("{0}", ExistingUser.Id),
                Encoding.UTF8,
                "application/json");
        };

        It should_return_bad_request = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_indicate_no_target = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.NoTarget);

        private static ScimGroup GroupToUpdate;
        private static ScimUser ExistingUser;
    }
}