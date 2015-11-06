namespace Owin.Scim.Tests.Integration.Users.Update.add
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
        static with_path_and_existing_multivaluedattribute_values()
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName(),
                Emails = new List<Email>
                {
                    new Email { Value = "user@corp.com", Type = "work" }
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
                            ""op"": ""add"",
                            ""path"": ""emails"",
                            ""value"": [{
                                ""value"": ""babs@jensen.org"",
                                ""type"": ""home""
                            }]
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_add_the_new_values = () => UpdatedUser
            .Emails
            .SingleOrDefault(e => e.Value.Equals("babs@jensen.org"))
            .ShouldNotBeNull();

        It should_contain_the_existing_values = () => UpdatedUser
            .Emails
            .SingleOrDefault(e => e.Value.Equals("user@corp.com"))
            .ShouldNotBeNull();

        It should_append_the_new_attribute_values = () => UpdatedUser
            .Emails
            .Count()
            .ShouldEqual(2);
    }
}