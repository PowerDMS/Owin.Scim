namespace Owin.Scim.Tests.Integration.Users.Update.replace
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
                    new PhoneNumber {Value = "8009991234", Type = "old"},
                    new PhoneNumber {Value = "8009991235", Type = "old2"}
                }
            };

            PatchContent = new StringContent(
                @"
                        {
                            ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                            ""Operations"": [{
                                ""op"":""replace"",
                                ""path"": ""displayName"",
                                ""value"": ""Daniel""
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""phoneNumbers"",
                                ""value"": [{""value"":""8887771234"", ""type"":""new""}]
                            },
                            {
                                ""op"":""replace"",
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

        /// <summary>
        /// Refer to https://tools.ietf.org/html/rfc7644#section-3.5.2.3
        /// "Sub-attributes that are not specified in the "value" parameter are left unchanged"
        /// </summary>
        It should_update_complex_attribute = () =>
        {
            UpdatedUser.Name.GivenName.ShouldEqual("Daniel");
            UpdatedUser.Name.FamilyName.ShouldEqual("Regular Joe");
        };

        It should_replace_multi_attribute = () =>
        {
            UpdatedUser.PhoneNumbers.Count().ShouldEqual(1);
            UpdatedUser.PhoneNumbers.First().Type.ShouldEqual("new");
        };
    }
}