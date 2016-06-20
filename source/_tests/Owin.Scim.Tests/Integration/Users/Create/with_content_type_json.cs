namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;
    using Model.Users;

    using v2.Model;

    /// <summary>
    /// my guess is that we want to still allow content-type of json
    /// </summary>
    public class with_content_type_json : using_a_scim_server
    {
        Establish context = () =>
        {
            UserDto = new ScimUser2()
            {
                UserName = UserNameUtility.GenerateUserName()
            };
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PostAsync("v2/users", new ObjectContent<ScimUser>(UserDto, new JsonMediaTypeFormatter()))
                .Result;

            StatusCode = Response.StatusCode;
        };

        It should_return_created = () => StatusCode.ShouldEqual(HttpStatusCode.Created);

        protected static ScimUser UserDto;

        protected static HttpResponseMessage Response;

        protected static HttpStatusCode StatusCode;
    }
}