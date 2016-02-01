namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Net;

    using Machine.Specifications;

    using Model.Users;

    public class with_no_patch_request : when_updating_a_user
    {
        Establish context = () =>
        {
            UserToUpdate = new User
            {
                UserName = UserNameUtility.GenerateUserName()
            };

            PatchContent = null;
        };

        It should_return_an_error = () => PatchResponse.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);
    }
}