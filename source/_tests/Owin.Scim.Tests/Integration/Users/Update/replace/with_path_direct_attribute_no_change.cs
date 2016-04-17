namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System;
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
                    new PhoneNumber {Value = "8009991234", Type = "old", Ref = new Uri("http://hello.org/world")}
                }
            };

            PatchContent = new StringContent(
                @"
                        {
                            ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                            ""Operations"": [{
                                ""op"":""replace"",
                                ""path"": ""displayName"",
                                ""value"": ""Danny""
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""name.GivenName"",
                                ""value"": null
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""name.FamilyName"",
                                ""value"": ""Regular Joe""
                            },
                            {
                                ""op"":""replace"",
                                ""path"": ""phoneNumbers[type ne \""new\""].$ref"",
                                ""value"": ""http://hello.org/world""
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_update_version = () => UpdatedUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);

        It should_not_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldEqual(UserToUpdate.Meta.LastModified);

        It should_not_modify_complex_attribute = () => UpdatedUser.Name.ShouldBeLike(UserToUpdate.Name);

        It should_not_modify_multi_attribute = () => UpdatedUser.PhoneNumbers.ShouldBeLike(UserToUpdate.PhoneNumbers);
    }
}