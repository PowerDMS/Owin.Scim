namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_no_path : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new User
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
                                ""op"":""add"",
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

        It should_add_the_email = () => UpdatedUser
            .Emails
            .SingleOrDefault(e => e.Value.Equals("babs@jensen.org"))
            .ShouldNotBeNull();

        It should_add_phone = () =>
        {
            UpdatedUser.PhoneNumbers.Count().ShouldBeGreaterThan(1);
            UpdatedUser.PhoneNumbers.First().Type.ShouldEqual("old");
            UpdatedUser.PhoneNumbers.Last().Type.ShouldEqual("new");
        };

        It should_replace_the_display_name = () => UpdatedUser.DisplayName.ShouldEqual("Babs");

        It should_replace_complex_attribute = () =>
        {
            UpdatedUser.Name.GivenName.ShouldEqual("Daniel");
            UpdatedUser.Name.FamilyName.ShouldBeNull();
        };
    }
}