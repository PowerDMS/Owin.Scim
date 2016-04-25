namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    public class equals_with_string : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen", Active = true },
                new User { UserName = "LSmith", Active = true }
            };

            FilterExpression = "userName eq \"bjensen\"";
        };

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }
}