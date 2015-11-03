namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    public class equals_with_boolean : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen", Active = false },
                new User { UserName = "LSmith", Active = true }
            };

            FilterExpression = new ScimFilter("active eq \"true\"");
        };

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }
}