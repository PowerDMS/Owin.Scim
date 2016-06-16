namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_path_and_filter_no_change : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                Emails = new List<Email>
                {
                    new Email { Value = "user@corp.com", Type = "work" },
                    new Email { Value = "user@gmail.com", Type = "home", Primary = true }
                },
                PhoneNumbers = new[]
                {
                    new PhoneNumber {Value = "8009991234", Type = "old"},
                    new PhoneNumber {Value = "8009991235", Type = "old"}
                }
            };

            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"": ""replace"",
                            ""path"": ""emails[type eq \""work\""]"",
                            ""value"": {
                                ""value"": ""user@corp.com""
                            }
                        },
                        {
                            ""op"": ""replace"",
                            ""path"": ""phoneNumbers[type eq \""old\""]"",
                            ""value"": {
                                ""primary"": ""false""
                            }
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_update_version = () => UpdatedUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);

        It should_not_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldEqual(UserToUpdate.Meta.LastModified);

        It should_not_update_complex_attributes = () =>
        {
            UpdatedUser.Emails.ShouldBeLike(UserToUpdate.Emails);
            UpdatedUser.PhoneNumbers.ShouldBeLike(UserToUpdate.PhoneNumbers);
        };
    }
}