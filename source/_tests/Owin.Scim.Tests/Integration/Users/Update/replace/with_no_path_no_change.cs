namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_no_path_no_change : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                DisplayName = "Babs",
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
                                    ""phonenumbers"":[{
                                        ""value"": ""8009991234"",
                                        ""type"": ""old""
                                    }],
                                    ""displayName"": ""Babs"",
                                    ""name"":{
                                        ""familyName"":""Regular Joe""
                                    }
                                }
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_not_change_version = () => 
            UpdatedUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);

        It should_not_change_last_modified = () =>
            UpdatedUser.Meta.LastModified.ShouldEqual(UserToUpdate.Meta.LastModified);

        It should_not_change_complex_attribute = () => UpdatedUser.Name.ShouldBeLike(UserToUpdate.Name);

        It should_not_change_multi_attribute = () => UpdatedUser.PhoneNumbers.ShouldBeLike(UserToUpdate.PhoneNumbers);
    }
}