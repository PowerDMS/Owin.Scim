namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_path_direct_attribute : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser
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
                                ""value"": ""Daniel""
                            },
                            {
                                ""op"":""add"",
                                ""path"": ""phoneNumbers"",
                                ""value"": [{""value"":""8887771234"", ""type"":""new""}]
                            },
                            {
                                ""op"":""add"",
                                ""path"": ""name"",
                                ""value"": {""givenName"":""Daniel""}
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_replace_simple_attribute = () => UpdatedUser.DisplayName.ShouldEqual("Daniel");

        It should_replace_complex_attribute = () =>
        {
            UpdatedUser.Name.GivenName.ShouldEqual("Daniel");
            UpdatedUser.Name.FamilyName.ShouldBeNull();
        };

        It should_append_multi_attribute = () =>
        {
            UpdatedUser.PhoneNumbers.Count().ShouldBeGreaterThan(1);
            UpdatedUser.PhoneNumbers.First().Type.ShouldEqual("old");
            UpdatedUser.PhoneNumbers.Last().Type.ShouldEqual("new");
        };
    }
}