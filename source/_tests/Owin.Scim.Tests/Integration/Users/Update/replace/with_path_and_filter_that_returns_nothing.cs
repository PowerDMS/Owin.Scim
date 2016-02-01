namespace Owin.Scim.Tests.Integration.Users.Update.replace
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_path_and_filter_that_returns_nothing : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName(),
                Emails = new List<Email>
                {
                    new Email { Value = "Babs@Jensen.org", Type = "home" }
                }
            };

            PatchContent = new StringContent(
                @"{
                    ""schemas"": [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                    ""Operations"":[{
                        ""op"":""replace"",
                        ""path"":""emails[type eq \""work\""]"",
                        ""value"": {
                            ""value"":""babs@company.org""
                        }
                    }]
                }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_an_error = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_no_target = async () => (await PatchResponse.Content.ReadAsAsync<IEnumerable<ScimError>>())
            .Single()
            .ScimErrorType
            .ShouldEqual(ScimErrorType.NoTarget);

        protected static string UserId;
    }
}