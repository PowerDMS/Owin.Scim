namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class or_present_equals : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser2 { UserName = "BJensen"},
                new ScimUser2 { UserName = "LSmith", Title = "Engineer", UserType = "Manager" },
                new ScimUser2 { UserName = "DGioulakis", UserType = "Employee" }
            };

            FilterExpression = "title pr or userType eq \"employee\"";
        };

        It should_filter = () => Users.Where(Predicate).Select(u => u.UserName).ShouldContainOnly("LSmith", "DGioulakis");
    }
}