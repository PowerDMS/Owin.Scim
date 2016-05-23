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
            var groups = new List<ScimGroup>
            {
                new ScimGroup
                {
                    DisplayName = "dev1"
                },
                new ScimGroup
                {
                    DisplayName = "dev2"
                },
                new ScimGroup
                {
                    DisplayName = "dev3"
                },
                new ScimGroup
                {
                    DisplayName = "sales1"
                },
                new ScimGroup
                {
                    DisplayName = "sales2"
                },
                new ScimGroup
                {
                    DisplayName = "hr"
                },
                new ScimGroup
                {
                    DisplayName = "exec"
                }
            };

            foreach (var group in groups)
            {
                await Server
                    .HttpClient
                    .PostAsync("groups", new ScimObjectContent<ScimGroup>(group))
                    .AwaitResponse()
                    .AsTask;
            }

            QueryString = "filter=displayName sw \"dev\"";
        };

        It should_return_groups_which_satisfy_the_filter = () => ListResponse.Resources.Count().ShouldEqual(3);
    }
}