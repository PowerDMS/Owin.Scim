namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using Scim.Querying;

    public class greaterthan_with_string : when_parsing_a_filter_expression<User>
    {
        Establish context = () =>
        {
            Users = new List<User>
            {
                new User {UserName = "DGioulakis" },
                new User { UserName = "BJensen", Addresses = new List<Address> { new Address { PostalCode = "10010" } } },
                new User { UserName = "ROMalley", Addresses = new List<Address> { new Address { PostalCode = "20005" } } }
            };

            FilterExpression = new ScimFilter("addresses.postalCode gt \"20000\"");
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("ROMalley");
    }
}