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
            var existingUser = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            Response = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<User>(existingUser, new ScimJsonMediaTypeFormatter()))
                .Result;

            UserDto = Response.Content.ReadAsAsync<User>(ScimJsonMediaTypeFormatter.AsArray()).Result;

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
                .PutAsync("users/" + UserDto.Id, new ObjectContent<User>(UserDto, new ScimJsonMediaTypeFormatter()))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;

            CreatedUser = Response.StatusCode == HttpStatusCode.OK
                ? JsonConvert.DeserializeObject<User>(bodyText)
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

        protected static User UserDto;

        protected static User CreatedUser;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}