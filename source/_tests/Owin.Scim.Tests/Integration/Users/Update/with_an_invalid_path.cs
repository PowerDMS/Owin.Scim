namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_an_invalid_path : using_a_scim_server
    {
        Establish context = async () =>
        {
            var userName = UserNameUtility.GenerateUserName();
            var existingUser = new User
            {
                UserName = userName
            };

            // Insert the first user so there's one already in-memory.
            var userRecord = await Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(existingUser, new JsonMediaTypeFormatter()))
                .AwaitResponse()
                .AsTask;

            UserId = (await userRecord.Content.ReadAsAsync<User>()).Id;
        };

        Because of = async () =>
        {
            // Below, the path is invalid because there is no property 'preferred'
            Response = await Server
                .HttpClient
                .SendAsync(
                    new HttpRequestMessage(
                        new HttpMethod("PATCH"), "users/" + UserId)
                    {
                        Content = new StringContent(
                            @"{ ""schemas"":
                               [""urn:ietf:params:scim:api:messages:2.0:PatchOp""],
                             ""Operations"":[
                               {
                                   ""op"":""add"",
                                   ""path"":""/preferred"",
                                   ""value"": ""en-US""
                               }]
                           }", 
                            Encoding.UTF8, 
                            "application/json")
                    })
                .AwaitResponse()
                .AsTask;
        };

        It should_return_an_error = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_path = async () => (await Response.Content.ReadAsAsync<IEnumerable<ScimError>>())
            .Single()
            .ScimType
            .ShouldEqual(ScimType.InvalidPath);

        protected static string UserId;

        protected static HttpResponseMessage Response;
    }
}