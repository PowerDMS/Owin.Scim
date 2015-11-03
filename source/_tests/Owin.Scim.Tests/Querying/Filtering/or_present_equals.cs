namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    public class or_present_equals : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen"},
                new User { UserName = "LSmith", Title = "Engineer", UserType = "Manager" },
                new User { UserName = "DGioulakis", UserType = "Employee" }
            };

            FilterExpression = new ScimFilter("title pr or userType eq \"employee\"");
        };

        It should_filter = () => Users.Where(Predicate).Select(u => u.UserName).ShouldContainOnly("LSmith", "DGioulakis");
    }
}