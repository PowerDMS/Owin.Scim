namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_no_path : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
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
                                ""op"":""replace"",
                                ""value"": {
                                    ""emails"":[{
                                        ""value"": ""babs@jensen.org"",
                                        ""type"": ""home""
                                    }],
                                    ""phonenumbers"":[{
                                        ""value"": ""8885551234"",
                                        ""type"": ""new""
                                    }],
                                    ""displayName"": ""Babs"",
                                    ""name"":{
                                        ""honorificPrefix"":""Dr"",
                                        ""givenName"":""Daniel""
                                    }
                                }
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_update_version = () => UpdatedUser.Meta.Version.ShouldNotEqual(UserToUpdate.Meta.Version);

        It should_update_last_modified = () => UpdatedUser.Meta.LastModified.ShouldBeGreaterThan(UserToUpdate.Meta.LastModified);

        It should_replace_email_list = () => UpdatedUser
            .Emails
            .SingleOrDefault(e => e.Value.Equals("babs@jensen.org"))
            .ShouldNotBeNull();

        It should_replace_phone_list = () =>
        {
            UpdatedUser.PhoneNumbers.Count().ShouldEqual(1);
            UpdatedUser.PhoneNumbers.First().Type.ShouldEqual("new");
        };

        It should_replace_the_display_name = () => UpdatedUser.DisplayName.ShouldEqual("Babs");

        It should_update_complex_attribute = () =>
        {
            UpdatedUser.Name.GivenName.ShouldEqual("Daniel");
            UpdatedUser.Name.FamilyName.ShouldEqual("Regular Joe");
        };
    }
}