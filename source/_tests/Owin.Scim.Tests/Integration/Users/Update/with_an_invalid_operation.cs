namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_an_invalid_operation : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            PatchContent = new StringContent(
                @"{ ""schemas"":
                               [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                             ""Operations"":[
                               {
                                   ""op"":""bogusOp"",
                                   ""path"":""preferredLanguage"",
                                   ""value"": ""en-US""
                               }]
                           }",
                Encoding.UTF8,
                "application/json");
        };

        It should_return_an_error = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_syntax = () =>
        {
            var error = PatchResponse.Content.ReadAsAsync<ScimError>().Result;

            error.ScimType.ShouldEqual(ScimErrorType.InvalidSyntax);
        };

        protected static string UserId;
    }
}