namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_missing_schema : using_a_scim_server
    {
        Establish context = () =>
        {
            UserDto = new UserWithNoSchema()
            {
                UserName = "Oops"
            };
        };

        Because of = () =>
        {
            Response = Server
                .HttpClient
                .PostAsync("users", new ScimObjectContent<UserWithNoSchema>(UserDto))
                .Result;

            StatusCode = Response.StatusCode;

            Error = StatusCode != HttpStatusCode.BadRequest ? null : Response.Content
                .ScimReadAsAsync<IEnumerable<ScimError>>()
                .Result
                .Single();
        };

        It should_return_bad_request = () => StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_value = () => Error?.ScimType.ShouldEqual(ScimErrorType.InvalidValue);

        It should_return_error_schema = () => Error?.Schemas.ShouldContain(ScimConstants.Messages.Error);

        protected static UserWithNoSchema UserDto;

        protected static HttpResponseMessage Response;

        protected static HttpStatusCode StatusCode;

        protected static ScimError Error;

        public class UserWithNoSchema
        {
            public string UserName { get; set; }
            public string ExternalId { get; set; }
        }
    }
}