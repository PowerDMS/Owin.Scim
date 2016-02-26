namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_path_direct_attribute_no_change : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new User
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
                                ""op"":""add"",
                                ""path"": ""displayName"",
                                ""value"": ""Danny""
                            },
                            {
                                ""op"":""add"",
                                ""path"": ""name"",
                                ""value"": {
                                    ""familyName"": ""Regular Joe""
                                }
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_update_version = () => UpdatedUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);

        It should_not_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldEqual(UserToUpdate.Meta.LastModified);

        It should_not_change_complex_attribute = () => UpdatedUser.Name.ShouldBeLike(UserToUpdate.Name);
    }
}