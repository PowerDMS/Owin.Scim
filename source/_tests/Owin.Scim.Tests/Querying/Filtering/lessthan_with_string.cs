namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    public class lessthan_with_string : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser { UserName = "abcdef" },
                new ScimUser { UserName = "acdefg" }
            };

            FilterExpression = "userName lt \"acbdef\"";
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("abcdef");
    }
}