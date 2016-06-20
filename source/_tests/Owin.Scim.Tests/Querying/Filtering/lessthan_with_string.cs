namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class lessthan_with_string : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser2 { UserName = "abcdef" },
                new ScimUser2 { UserName = "acdefg" }
            };

            FilterExpression = "userName lt \"acbdef\"";
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("abcdef");
    }
}