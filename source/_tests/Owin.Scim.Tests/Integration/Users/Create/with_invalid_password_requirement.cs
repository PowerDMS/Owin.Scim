namespace Owin.Scim.Tests.Integration.Users.Create
{
    using System.Net;

    using Machine.Specifications;

    using Model;
    using Model.Users;

    using v2.Model;

    public class with_invalid_password_requrement : when_creating_a_user
    {
        Establish context = () =>
        {
            UserDto = new ScimUser2
            {
                UserName = UserNameUtility.GenerateUserName(),
                Password = "short"
            };
        };

        It should_return_bad_request =
            () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_value =
            () =>
            {
                Error.ScimType.ShouldEqual(ScimErrorType.InvalidValue);
                Error.Detail.ShouldContain("password");
            };
    }
}