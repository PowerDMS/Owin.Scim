namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    public class with_path_and_null_multivaluedattribute : when_updating_a_user
    {
        static with_path_and_null_multivaluedattribute()
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
                            ""op"": ""replace"",
                            ""path"": ""emails""
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_ignore_the_null_attibute = () => UpdatedUser
            .Emails
            .ShouldBeNull();
    }
}