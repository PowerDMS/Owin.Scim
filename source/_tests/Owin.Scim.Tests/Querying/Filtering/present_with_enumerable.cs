namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;
    
    using Model.Users;

    public class present_with_enumerable : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen" },
                new User { UserName = "LSmith", Emails = new List<Email>() },
                new User { UserName = "DGioulakis", Emails = new List<Email> { new Email { Value = "my@email.com", Primary = true, Type = "work" } } }
            };

            FilterExpression = "emails pr";
        };

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }
}