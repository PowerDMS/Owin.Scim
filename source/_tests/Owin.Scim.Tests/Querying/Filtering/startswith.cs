namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    public class startswith : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen", Active = true },
                new User { UserName = "LSmith", Active = true }
            };

            FilterExpression = new ScimFilter("userName sw \"lsm\"");
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("LSmith");
    }
}