namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_path_and_filter : when_updating_a_user
    {
        static with_path_and_filter()
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName(),
                Emails = new List<Email>
                {
                    new Email { Value = "user@corp.com", Type = "work" },
                    new Email { Value = "user@gmail.com", Type = "home", Primary = true }
                }
            };
        }

        Establish context = () =>
        {
            PatchContent = new StringContent(
                @"
                    {
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"": ""replace"",
                            ""path"": ""emails[type eq \""work\""]"",
                            ""value"": {
                                ""value"": ""user@employer.org"",
                                ""type"": ""work""
                            }
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_replace_the_multivaluedattribute = () => UpdatedUser
            .Emails
            .Single(e => e.Type.Equals("work"))
            .Value
            .ShouldEqual("user@employer.org");
    }
}