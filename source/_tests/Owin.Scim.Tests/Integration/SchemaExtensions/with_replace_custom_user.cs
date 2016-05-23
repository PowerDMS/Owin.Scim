namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    using Newtonsoft.Json;

    using Users;

    public class with_replace_custom_user : using_a_scim_server
    {
        private Establish context = () =>
        {
            var existingUser = new ScimUser
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            Response = Server
                .HttpClient
                .PostAsync("users", new ScimObjectContent<ScimUser>(existingUser))
                .Result;

            UserDto = Response.Content.ScimReadAsAsync<ScimUser>().Result;

            UserDto.Extension<EnterpriseUserExtension>().Department = "Sales";
            UserDto.AddExtension(
                new MyUserSchema
                {
                    Guid = "anything",
                    EnableHelp = true,
                    EndDate = DateTime.Today.ToUniversalTime(),
                    ComplexData = new MyUserSchema.MySubClass
                    {
                        DisplayName = "hello",
                        Value = "world"
                    }
                });
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PutAsync("users/" + UserDto.Id, new ScimObjectContent<ScimUser>(UserDto))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;

            CreatedUser = Response.StatusCode == HttpStatusCode.OK
                ? Response.Content.ScimReadAsAsync<ScimUser>().Result
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? JsonConvert.DeserializeObject<ScimError>(bodyText)
                : null;
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.OK);

        It should_return_new_version = () => CreatedUser.Meta.Version.ShouldNotEqual(UserDto.Meta.Version);

        It should_return_enterprise_user = () =>
            CreatedUser
                .Extension<EnterpriseUserExtension>()
                .ShouldBeLike(UserDto.Extension<EnterpriseUserExtension>());

        It should_return_custom_schema = () =>
            CreatedUser
                .Extension<MyUserSchema>()
                .ShouldBeLike(UserDto.Extension<MyUserSchema>());

        protected static ScimUser UserDto;

        protected static ScimUser CreatedUser;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}