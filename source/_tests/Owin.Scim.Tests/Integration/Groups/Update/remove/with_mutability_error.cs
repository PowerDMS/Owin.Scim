namespace Owin.Scim.Tests.Integration.Groups.Update.remove
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;
    using Model.Groups;

    using v2.Model;

    public class with_mutability_error : when_updating_a_group
    {
        Establish context = () =>
        {
            ExistingUser = CreateUser(new ScimUser2 {UserName = Users.UserNameUtility.GenerateUserName()});
            var anotherUser = CreateUser(new ScimUser2 {UserName = Users.UserNameUtility.GenerateUserName()});
            GroupToUpdate = CreateGroup(
                new ScimGroup2
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
                            ""path"": ""members[type eq \""user\""].value"",
                        }]
                    }";

            PatchContent = new StringContent(
                stringFormat.Replace("{0}", ExistingUser.Id),
                Encoding.UTF8,
                "application/json");
        };

        It should_return_bad_request = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_indicate_mutability = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.Mutability);

        private static ScimGroup GroupToUpdate;
        private static ScimUser ExistingUser;
    }
}