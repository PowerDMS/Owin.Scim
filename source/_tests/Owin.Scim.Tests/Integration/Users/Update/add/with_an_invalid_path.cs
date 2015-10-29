namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_an_invalid_path : when_updating_a_user
    {
        static with_an_invalid_path()
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };
        }

        Establish context = () =>
        {
            PatchContent = new StringContent(
                @"{ ""schemas"":
                               [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                             ""Operations"":[
                               {
                                   ""op"":""add"",
                                   ""path"":""preferred"",
                                   ""value"": ""en-US""
                               }]
                           }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_an_error = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_path = async () => (await PatchResponse.Content.ReadAsAsync<IEnumerable<ScimError>>())
            .Single()
            .ScimType
            .ShouldEqual(ScimType.InvalidPath);

        protected static string UserId;
    }
}