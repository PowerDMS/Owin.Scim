namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    using v2.Model;

    [Subject(typeof(ScimFilterVisitor<>))]
    public class and_equals_braces : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser2 { UserName = "BJensen", UserType = "employee"},
                new ScimUser2
                {
                    UserName = "ROMalley",
                    UserType = "employee",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@example.com", Type = "work" }
                    }
                },
                new ScimUser2
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