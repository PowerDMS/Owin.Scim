using System.Linq;
using System.Net.Http;

namespace Owin.Scim.Tests.Integration.Users.Update.add
{
    using System.Collections.Generic;
    using System.Net;

    using Machine.Specifications;

    using Model;
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

        It should_return_invalid_syntax = () =>
        {
            var error = PatchResponse.Content.ReadAsAsync<ScimError>().Result;

            error.ScimType.ShouldEqual(ScimErrorType.InvalidSyntax);
        };
    }
}