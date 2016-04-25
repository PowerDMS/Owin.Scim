namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    [Subject(typeof(ScimFilterVisitor<>))]
    public class and_equals_braces : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen", UserType = "employee"},
                new User
                {
                    UserName = "ROMalley",
                    UserType = "employee",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@example.com", Type = "work" }
                    }
                },
                new User
                {
                    UserName = "DGioulakis",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@corp.com", Type = "work" },
                        new Email { Value = "user2@example.com", Type = "home" }
                    }
                }
            };

            FilterExpression = "userType eq \"Employee\" and (emails.type eq \"work\")";
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("ROMalley");
    }
}