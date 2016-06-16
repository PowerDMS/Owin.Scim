namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class with_path_no_target : when_updating_a_user
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
                }
            };

            PatchContent = new StringContent(
                @"{
                        ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                        ""Operations"": [{
                            ""op"": ""replace"",
                            ""path"": ""emails[type eq \""cell\""]"",
                            ""value"": { ""value"": ""romalley@email.com"" }
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        /*
        o  If the target location is a multi-valued attribute for which a
           value selection filter ("valuePath") has been supplied and no
           record match was made, the service provider SHALL indicate failure
           by returning HTTP status code 400 and a "scimType" error code of
           "noTarget".
        */
        It should_return_bad_request = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_no_target = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.NoTarget);
    }
}