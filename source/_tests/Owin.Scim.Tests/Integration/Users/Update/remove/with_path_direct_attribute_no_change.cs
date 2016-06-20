namespace Owin.Scim.Tests.Integration.Users.Update.remove
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_path_direct_attribute_no_change : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                DisplayName = "Danny",
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
                                ""path"": ""externalId""
                            },
                            {
                                ""op"":""remove"",
                                ""path"": ""name""
                            },
                            {
                                ""op"":""remove"",
                                ""path"": ""emails""
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldEqual(UserToUpdate.Meta.LastModified);
    }
}