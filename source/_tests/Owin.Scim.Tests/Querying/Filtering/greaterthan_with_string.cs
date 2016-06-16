namespace Owin.Scim.Tests.Querying.Filtering
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Users;

    using v2.Model;

    public class greaterthan_with_string : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser2 {UserName = "DGioulakis" },
                new ScimUser2 { UserName = "BJensen", Addresses = new List<MailingAddress> { new MailingAddress { PostalCode = "10010" } } },
                new ScimUser2 { UserName = "ROMalley", Addresses = new List<MailingAddress> { new MailingAddress { PostalCode = "20005" } } }
            };

            FilterExpression = "addresses.postalCode gt \"20000\"";
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("ROMalley");
    }
}