using System.Linq;

namespace Owin.Scim.Tests.Integration.Users.Update.multiOperation
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_only_one_valid_change : when_updating_a_user
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
                                ""op"":""remove"",
                                ""path"": ""name.givenName""
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""name.familyName"",
                                ""value"": ""Dude""
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""phoneNumbers[type eq \""old\""].value"",
                                ""value"": ""8009991234""
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_have_updated_attribute = () => UpdatedUser.Name.FamilyName.ShouldEqual("Dude");

        It should_not_have_updated_multi_attribute = () => UpdatedUser.PhoneNumbers.ShouldBeLike(UserToUpdate.PhoneNumbers);
    }
}