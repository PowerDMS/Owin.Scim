namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    using Newtonsoft.Json;

    using Users;

    using v2.Model;

    public class with_add_custom_user_validation : using_a_scim_server
    {
        Establish context = () =>
        {
            UserDto = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            UserDto.Extension<MyUserSchema>().Ref = @"\\badUri";
            UserDto.Extension<MyUserSchema>().ComplexData = new MyUserSchema.MySubClass
            {
                DisplayName = "hello",
                Value = "world"
            };
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PostAsync("v2/users", new ScimObjectContent<ScimUser>(UserDto))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;

            CreatedUser = Response.StatusCode == HttpStatusCode.Created
                ? Response.Content.ScimReadAsAsync<ScimUser2>().Result
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? JsonConvert.DeserializeObject<ScimError>(bodyText)
                : null;
        };

        It should_return_badrequest = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_syntax = () => Error.ScimType.ShouldEqual(ScimErrorType.InvalidSyntax);

        It should_return_detail = () => Error.Detail.ShouldContain("$ref");

        protected static ScimUser UserDto;

        protected static ScimUser CreatedUser;

        protected static HttpResponseMessage Response;

        protected static ScimError Error;
    }
}