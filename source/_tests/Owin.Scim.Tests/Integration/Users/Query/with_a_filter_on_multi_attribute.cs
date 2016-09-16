namespace Owin.Scim.Tests.Integration.Users.Query
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    [Ignore("Causes exception")]
    public class with_a_filter_on_multi_attribute : when_querying_users
    {
        Establish context = async () =>
        {
            var users = new List<ScimUser>
            {
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name {GivenName = "Daniel", FamilyName = "Gioulakis"},
                    Emails = new[] {new Email {Value = "danny@dude.com", Type = "main"}}
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name {GivenName = "Lev", FamilyName = "Myshkin"},
                    Emails = new[] {new Email {Value = "lev@dude.com"}}
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name {GivenName = "Nastasya", FamilyName = "Barashkova"}
                }
            };

            foreach (var user in users)
            {
                await Server
                    .HttpClient
                    .PostAsync("v2/users", new ScimObjectContent<ScimUser>(user))
                    .AwaitResponse()
                    .AsTask;
            }

            QueryString = "filter=emails[type eq \"main\"]";
        };

        It should_return_users_which_satisfy_the_filter = () => ListResponse.Resources.Count().ShouldEqual(1);
    }
}