namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    public class with_missing_username : when_creating_a_user
    {
        Establish context = () =>
        {
            UserDto = new User
            {
                UserName = null
            };
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_contain_two_errors = () =>
        {
            var content = Response.Content.ReadAsAsync<IEnumerable<ScimError>>(ScimJsonMediaTypeFormatter.AsArray()).Result;
            content.Count().ShouldEqual(2);
        };
    }
}