namespace Owin.Scim.Tests.Integration.Users.Update.multiOperation
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_failed_second_operation : when_updating_a_user_with_server_state
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
                                ""op"":""replace"",
                                ""path"": ""displayName"",
                                ""value"": ""Daniel""
                            },
                            {
                                ""op"":""remove""
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

        It should_return_bad_request = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_not_change_resource = () => ServerUser.Meta.Version.ShouldEqual(UserToUpdate.Meta.Version);
    }
}