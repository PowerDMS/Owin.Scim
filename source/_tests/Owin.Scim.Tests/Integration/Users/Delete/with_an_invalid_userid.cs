namespace Owin.Scim.Tests.Integration.Users.Delete
{
    using System.Net;

    using Machine.Specifications;

    public class with_an_invalid_userid : when_deleting_a_user
    {
        Establish context = () => UserId = "invalid";

        It should_return_not_found = () => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
    }
}