namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Configuration;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    [Subject(typeof(ScimFilterVisitor<>))]
    public class and_equals_contains_brackets : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser { UserName = "BJensen", UserType = "employee"},
                new ScimUser
                {
                    UserName = "ROMalley",
                    UserType = "manager",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@example.com", Type = "work" }
                    }
                },
                new ScimUser
                {
                    UserName = "DGioulakis",
                    UserType = "employee",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user2@mymail.com", Type = "home" },
                        new Email { Value = "user@example.com", Type = "work" }
                    }
                }
            };

            FilterExpression = "userType eq \"Employee\" and emails[type eq \"work\" and value co \"@example.com\"]";
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("DGioulakis");
    }
}