namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_path_and_existing_multivaluedattribute_values : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName(),
                Emails = new List<Email>
                {
                    new Email { Value = "user@corp.com", Type = "work" }
                }
            };

            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"": ""replace"",
                            ""path"": ""emails"",
                            ""value"": [{
                                ""value"": ""babs@jensen.org"",
                                ""type"": ""home""
                            },{
                                ""value"": ""babs@corp.org"",
                                ""type"": ""work""
                            }]
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_replace_the_entire_multivaluedattribute = () => UpdatedUser
            .Emails
            .Select(e => e.Value)
            .ShouldContainOnly("babs@jensen.org", "babs@corp.org");
    }
}