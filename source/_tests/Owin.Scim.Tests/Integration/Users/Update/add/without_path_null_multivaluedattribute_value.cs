namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    [Tags("no_path")]
    public class without_path_null_multivaluedattribute_value : when_updating_a_user
    {
        static without_path_null_multivaluedattribute_value()
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName(),
                Emails = null
            };
        }

        Establish context = () =>
        {
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
                                    }]
                                }
                            }]
                        }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_add_the_email = () => UpdatedUser
            .Emails
            .Where(e => e.Value.Equals("babs@jensen.org"))
            .ShouldNotBeEmpty();
        
        It should_replace_the_attribute_value = () => UpdatedUser
            .Emails
            .Count()
            .ShouldEqual(1);

        protected static string UserId;
    }
}