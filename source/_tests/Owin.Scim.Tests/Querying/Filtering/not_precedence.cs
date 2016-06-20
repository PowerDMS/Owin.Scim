namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    using v2.Model;

    [Subject(typeof(ScimFilterVisitor<>))]
    public class not_precedence : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser2
                {
                    UserName = "BJensen",
                    UserType = "employee",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@example.com", Type = "work" }
                    }
                },
                new ScimUser2
                {
                    UserName = "ROMalley",
                    UserType = "manager",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@example.com", Type = "work" }
                    }
                },
                new ScimUser2
                {
                    UserName = "DGioulakis",
                    UserType = "employee",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@corp.com", Type = "work" },
                        new Email { Value = "user2@mail.com", Type = "home" }
                    }
                }
            };

            FilterExpression = "userType eq \"employee\" and not (emails co \"example.com\" or emails.value co \"example.org\")";
        };

        It should_filter = () => Users.Where(Predicate).Count().ShouldEqual(1);
    }
}