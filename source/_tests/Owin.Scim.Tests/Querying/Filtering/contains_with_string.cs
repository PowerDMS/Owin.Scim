namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;
    
    using Machine.Specifications;

    using Model.Users;

    public class contains_with_string : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser { UserName = "BJensen" },
                new ScimUser { UserName = "ROMalley", Name = new Name { FamilyName = "O'Malley", GivenName = "Ryan" } }
            };

            FilterExpression = "name.familyName co \"malley\"";
        };

        It should_filter = () => Users.SingleOrDefault(Predicate).ShouldNotBeNull();
    }
}