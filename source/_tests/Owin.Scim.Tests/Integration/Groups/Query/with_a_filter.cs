namespace Owin.Scim.Tests.Integration.Groups.Query
{
    using System.Collections.Generic;
    using System.Linq;

    using Machine.Specifications;

    using Model.Groups;

    using v2.Model;

    [Ignore("ListResponse needs a jsonconverter")]
    public class with_a_filter : when_querying_groups
    {
        Establish context = async () =>
        {
            var groups = new List<ScimGroup>
            {
                new ScimGroup2
                {
                    DisplayName = "dev1"
                },
                new ScimGroup2
                {
                    DisplayName = "dev2"
                },
                new ScimGroup2
                {
                    DisplayName = "dev3"
                },
                new ScimGroup2
                {
                    DisplayName = "sales1"
                },
                new ScimGroup2
                {
                    DisplayName = "sales2"
                },
                new ScimGroup2
                {
                    DisplayName = "hr"
                },
                new ScimGroup2
                {
                    DisplayName = "exec"
                }
            };

            foreach (var group in groups)
            {
                await Server
                    .HttpClient
                    .PostAsync("v2/groups", new ScimObjectContent<ScimGroup>(group))
                    .AwaitResponse()
                    .AsTask;
            }

            QueryString = "filter=displayName sw \"dev\"";
        };

        It should_return_groups_which_satisfy_the_filter = () => ListResponse.Resources.Count().ShouldEqual(3);
    }
}