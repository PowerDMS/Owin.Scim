namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    [Subject(typeof(ScimFilterVisitor<>))]
    public class or_brackets_equals_and_contains : when_parsing_a_filter_expression<User>
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
                        new Email { Value = "user@example.com", Type = "home" }
                    },
                    Ims = new List<InstantMessagingAddress>
                    {
                        new InstantMessagingAddress { Value = "romalley@foo.com", Type = "xmpp" }
                    }
                },
                new User
                {
                    UserName = "DGioulakis",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@example.com", Type = "work" },
                        new Email { Value = "user2@example.com", Type = "home" }
                    }
                }
            };

            FilterExpression = new ScimFilter(
                "emails[type eq \"work\" and value co \"@example.com\"] or ims[type eq \"xmpp\" and value co \"@foo.com\"]");
        };

        It should_filter = () => Users.Where(Predicate).Select(u => u.UserName).ShouldContainOnly("ROMalley", "DGioulakis");
    }
}