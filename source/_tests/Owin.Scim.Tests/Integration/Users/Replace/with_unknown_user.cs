namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;

    using Machine.Specifications;

    using Model.Users;

    public class with_unknown_user : when_replacing_a_user
    {
        Establish context = () =>
        {
            UserId = "bogus-user-id";

            UserDto = new User
            {
                Id = UserId,
                UserName = "bogus-user-name",
                PreferredLanguage = "en-US"
            };
        };

        It should_return_error = () =>
        {
            Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            var errors = Response.Content.ReadAsAsync<Model.ScimError[]>().Result;

            // only 400 returns scimType
            errors[0].ScimType.ShouldBeNull();

            errors[0].Detail.ShouldContain(UserId);
            errors[0].Detail.ShouldContain("not found");

            errors[0].Status.ShouldEqual(HttpStatusCode.NotFound);
        };
    }
}