namespace Owin.Scim.Tests.Integration.Users.Query
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    [Ignore("Need to figure out if full namespace is supported with filter")]
    public class with_a_filter_and_enterprise_user : when_querying_users
    {
        Establish context = async () =>
        {
            var users = new List<ScimUser>
            {
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Daniel", FamilyName = "Gioulakis" },
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Lev", FamilyName = "Myshkin" }
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Nastasya", FamilyName = "Barashkova" }
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Parfyón", FamilyName = "Rogózhin" }
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Iván", FamilyName = "Yepanchín" }
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Agláya", FamilyName = "Ivánovna" }
                },
                new ScimUser2
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Daniel", FamilyName = "Smith" }
                }
            };

            foreach (var user in users)
            {
                user.Extension<EnterpriseUser2Extension>().Department = "Development";

                await Server
                    .HttpClient
                    .PostAsync("v2/users", new ScimObjectContent<ScimUser>(user))
                    .AwaitResponse()
                    .AsTask;
            }

            QueryString = "filter=urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:Department eq \"Development\"";
        };

        It should_return_users_which_satisfy_the_filter = () => ListResponse.Resources.Count().ShouldEqual(7);
    }
}