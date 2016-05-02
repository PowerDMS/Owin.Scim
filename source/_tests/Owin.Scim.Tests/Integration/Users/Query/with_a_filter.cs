namespace Owin.Scim.Tests.Integration.Users.Query
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    public class with_a_filter : when_querying_users
    {
        Establish context = async () =>
        {
            var users = new List<User>
            {
                new User
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Daniel", FamilyName = "Gioulakis" }
                },
                new User
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Lev", FamilyName = "Myshkin" }
                },
                new User
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Nastasya", FamilyName = "Barashkova" }
                },
                new User
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Parfyón", FamilyName = "Rogózhin" }
                },
                new User
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Iván", FamilyName = "Yepanchín" }
                },
                new User
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Agláya", FamilyName = "Ivánovna" }
                },
                new User
                {
                    UserName = UserNameUtility.GenerateUserName(),
                    Name = new Name { GivenName = "Daniel", FamilyName = "Smith" }
                }
            };

            foreach (var user in users)
            {
                await Server
                    .HttpClient
                    .PostAsync("users", new ScimObjectContent<User>(user))
                    .AwaitResponse()
                    .AsTask;
            }

            QueryString = "filter=name.givenName eq \"daniel\"";
        };

        It should_return_users_which_satisfy_the_filter = () => ListResponse.Resources.Count().ShouldEqual(2);
    }
}