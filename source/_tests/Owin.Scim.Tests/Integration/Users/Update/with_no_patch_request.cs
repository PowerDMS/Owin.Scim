namespace Owin.Scim.Tests.Integration.Users.Update
{
    using System.Net;

    using Machine.Specifications;

    public class with_no_patch_request : when_updating_a_user
    {
        Establish context = () =>
        {
            PatchDocument = null;
        };

        It should_return_an_error = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);
    }
}