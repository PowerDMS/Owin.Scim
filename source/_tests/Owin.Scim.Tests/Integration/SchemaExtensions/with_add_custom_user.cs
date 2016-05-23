namespace Owin.Scim.Tests.Integration.SchemaExtensions
{
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    using Newtonsoft.Json;

    using Users;

    public class with_add_custom_user : using_a_scim_server
    {
        Establish context = () =>
        {
            UserDto = new ScimUser
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            UserDto.Extension<EnterpriseUserExtension>().Department = "Sales";
            UserDto.AddExtension(new MyUserSchema
            {
                Guid = "anything",
                Ref = "./users/1234",
                EnableHelp = true,
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
                .PostAsync("users", new ScimObjectContent<ScimUser>(UserDto))
                .Result;

            var bodyText = Response.Content.ReadAsStringAsync().Result;
            
            CreatedUser = Response.StatusCode == HttpStatusCode.Created
                ? Response.Content.ScimReadAsAsync<ScimUser>().Result
                : null;

            Error = Response.StatusCode == HttpStatusCode.BadRequest
                ? JsonConvert.DeserializeObject<ScimError>(bodyText)
                : null;
        };

        It should_return_created = () => Response.StatusCode.ShouldEqual(HttpStatusCode.Created);

        It should_return_the_user = () => CreatedUser.Id.ShouldNotBeEmpty();

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