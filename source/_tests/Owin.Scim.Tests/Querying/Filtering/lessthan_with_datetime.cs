namespace Owin.Scim.Tests.Querying.Filtering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;
    
    using Model.Users;

    public class lessthan_with_datetime : when_parsing_a_filter_expression<ScimUser>
    {
        Establish context = () =>
        {
            Users = new List<ScimUser>
            {
                new ScimUser { UserName = "BJensen", Meta = { LastModified = new DateTime(2015, 01, 01, 0, 0, 0, DateTimeKind.Utc) } },
                new ScimUser { UserName = "ROMalley", Meta = { LastModified = new DateTime(2014, 01, 01, 0, 0, 0, DateTimeKind.Utc) } }
            };

            FilterExpression = "meta.lastModified lt \"2014-05-13T04:42:34Z\"";
        };

        It should_filter = () => Users.Single(Predicate).UserName.ShouldEqual("ROMalley");
    }
}