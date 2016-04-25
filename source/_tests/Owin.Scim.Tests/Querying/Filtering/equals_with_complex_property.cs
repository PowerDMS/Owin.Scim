namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    public class equals_with_complex_property : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User { UserName = "BJensen", Active = true },
                new User { UserName = "ROMalley", Active = true, Name = new Name { FamilyName = "O'Malley", GivenName = "Ryan" } }
            };

            FilterExpression = "name.familyName eq \"O'Malley\"";
        };

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }
}