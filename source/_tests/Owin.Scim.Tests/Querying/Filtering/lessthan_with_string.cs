namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    public class lessthan_with_string : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "abcdef" },
                new User { UserName = "acdefg" }
            };

            FilterExpression = new ScimFilter("userName lt \"acbdef\"");
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("abcdef");
    }
}