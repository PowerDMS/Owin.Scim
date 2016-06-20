namespace Owin.Scim.Tests.Querying.Filtering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;
    
    using Model.Users;

    using v2.Model;

    public class greaterthanorequal_with_datetime : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser2 { UserName = "BJensen", Meta = { LastModified = new DateTime(2015, 01, 01, 0, 0, 0, DateTimeKind.Utc) } },
                new ScimUser2 { UserName = "ROMalley", Meta = { LastModified = new DateTime(2014, 01, 01, 0, 0, 0, DateTimeKind.Utc) } }
            };

            FilterExpression = "meta.lastModified ge \"2014-01-01T00:00:00Z\"";
        };

        It should_filter = () => Users.Count(Predicate).ShouldEqual(2);
    }
}