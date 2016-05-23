using System.Linq;

namespace Owin.Scim.Tests.Integration.Users.Update.multiOperation
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_update_after_adding_attribute : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser
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
                                ""op"":""add"",
                                ""path"": ""name"",
                                ""value"": {""givenName"":""Daniel""}
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""name.familyName"",
                                ""value"": ""Dude""
                            },
                            {
                                ""op"":""add"",
                                ""path"": ""phoneNumbers"",
                                ""value"": [{""value"":""8009991234"",""type"":""new""}]
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""phoneNumbers[type eq \""new\""].value"",
                                ""value"": ""8009991235""
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_have_added_attribute = () => UpdatedUser.Name.GivenName.ShouldEqual("Daniel");

        It should_have_updated_attribute = () => UpdatedUser.Name.FamilyName.ShouldEqual("Dude");

        It should_have_updated_multi_attribute = () => UpdatedUser.PhoneNumbers.Any(a => a.Value == "8009991235").ShouldBeTrue();
    }
}