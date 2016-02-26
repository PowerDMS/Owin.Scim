namespace Owin.Scim.Tests.Integration.Users.Update.remove
{
    using System.Collections.Generic;
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
                            ""op"": ""remove""
                        }]
                    }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_ok = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        /// <summary>
        /// Reference: https://tools.ietf.org/html/rfc7644#section-3.5.2.2
        /// </summary>
        It should_return_no_target = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.NoTarget);
    }
}