using System.Linq;

namespace Owin.Scim.Tests.Integration.Users.Update.multiOperation
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_undoing_changes : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                DisplayName = "Danny",
                Name = new Name
                {
                    FamilyName = "Regular Joe"
                },
                PhoneNumbers = new []
                {
                    new PhoneNumber {Value = "8009991234", Type = "old"}
                }
            };

            PatchContent = new StringContent(
                @"
                        {
                            ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                            ""Operations"": [{
                                ""op"":""remove"",
                                ""path"": ""displayName""
                            },
                            {
                                ""op"":""add"",
                                ""path"": ""name.givenName"",
                                ""value"": ""Dude""
                            },
                            {
                                ""op"":""add"",
                                ""path"": ""displayName"",
                                ""value"": ""Danny""
                            },
                            {
                                ""op"":""add"",
                                ""path"": ""emails"",
                                ""value"": [{""value"":""bad@one.cc"", ""type"":""new""}]
                            },
                            {
                                ""op"":""remove"",
                                ""path"": ""emails[type eq \""new\""]"",
                            },
                            {
                                ""op"":""remove"",
                                ""path"": ""name.givenName"",
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_change_resource = () => UpdatedUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);
    }
}