namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;
    
    using Model.Users;

    public class present_with_enumerable : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser { UserName = "BJensen" },
                new ScimUser { UserName = "LSmith", Emails = new List<Email>() },
                new ScimUser { UserName = "DGioulakis", Emails = new List<Email> { new Email { Value = "my@email.com", Primary = true, Type = "work" } } }
            };

            FilterExpression = "emails pr";
        };

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }
}