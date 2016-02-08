namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;

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

        It should_return_bad_request = 
            () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_value =
            () => Error.Single()
                    .ScimType
                    .ShouldEqual(ScimErrorType.InvalidValue);
    }
}