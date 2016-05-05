namespace Owin.Scim.Tests.Integration.Groups.Query
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Groups;
    using Model.Users;

    using Users;

    public class with_a_filter : when_querying_groups
    {
        Establish context = async () =>
        {
            var groups = new List<Group>
            {
                new Group
                {
                    DisplayName = "dev1"
                },
                new Group
                {
                    DisplayName = "dev2"
                },
                new Group
                {
                    DisplayName = "dev3"
                },
                new Group
                {
                    DisplayName = "sales1"
                },
                new Group
                {
                    DisplayName = "sales2"
                },
                new Group
                {
                    DisplayName = "hr"
                },
                new Group
                {
                    DisplayName = "exec"
                }
            };

            foreach (var group in groups)
            {
                await Server
                    .HttpClient
                    .PostAsync("groups", new ScimObjectContent<Group>(group))
                    .AwaitResponse()
                    .AsTask;
            }

            QueryString = "filter=displayName sw \"dev\"";
        };

        It should_return_groups_which_satisfy_the_filter = () => ListResponse.Resources.Count().ShouldEqual(3);
    }
}