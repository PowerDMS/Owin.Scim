namespace Owin.Scim.Tests.Integration.Users.Replace
{
    using System.Net;
    using System.Net.Http;
    using Machine.Specifications;

    using Model.Users;

    public class with_unknown_user : when_replacing_a_user
    {
        Establish context = () =>
        {
            UserId = "bogus-user-id";
            UserDto = new ScimUser
            {
                Id = UserId,
                UserName = "bogus-user-name",
                PreferredLanguage = "en-US"
            };
        };

        It should_return_error = () =>
        {
            Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
            var error = Response.Content.ScimReadAsAsync<Model.ScimError>().Result;

            // only 400 returns scimType
            error.ScimType.ShouldBeNull();

            error.Detail.ShouldContain(UserId);
            error.Detail.ShouldContain("not found");

            error.Status.ShouldEqual(HttpStatusCode.NotFound);
        };

        protected static string UserId;
    }
}