namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    [Subject(typeof(ScimFilterVisitor<>))]
    public class contains_with_multivaluedattribute_value : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen", UserType = "employee"},
                new User
                {
                    UserName = "ROMalley",
                    UserType = "manager",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@example.com", Type = "work" }
                    }
                },
                new User
                {
                    UserName = "DGioulakis",
                    UserType = "architect",
                    Emails = new List<Email>
                    {
                        new Email { Value = "user@corp.com", Type = "work" }
                    }
                }
            };

            FilterExpression = "userType ne \"Employee\" and not (emails co \"example.com\" or emails.value co \"example.org\"";
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("DGioulakis");
    }
}