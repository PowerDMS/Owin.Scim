namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_invalid_schema : using_a_scim_server
    {
        Establish context = () =>
        {
            UserDto = new UserWithSchema()
            {
                // older un-supported schema
                Schemas = new[] { @"urn:ietf:params:scim:schemas:core:1.0:User" },
                UserName = "Oops"
            };
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PostAsync("users", new ObjectContent<UserWithSchema>(UserDto, new ScimJsonMediaTypeFormatter()))
                .Result;

            StatusCode = Response.StatusCode;

            Error = StatusCode != HttpStatusCode.BadRequest ? null : Response.Content
                .ReadAsAsync<IEnumerable<ScimError>>(ScimJsonMediaTypeFormatter.AsArray())
                .Result
                .Single();
        };

        It should_return_bad_request = () => StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_value = () => Error?.ScimType.ShouldEqual(ScimErrorType.InvalidValue);

        It should_return_error_schema = () => Error?.Schemas.ShouldContain(ScimConstants.Messages.Error);

        protected static UserWithSchema UserDto;

        protected static HttpResponseMessage Response;

        protected static HttpStatusCode StatusCode;

        protected static ScimError Error;

        public class UserWithSchema
        {
            public string[] Schemas { get; set; }
            public string UserName { get; set; }
        }
    }
}