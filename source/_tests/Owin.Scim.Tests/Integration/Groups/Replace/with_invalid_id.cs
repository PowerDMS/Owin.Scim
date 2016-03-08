namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    public class with_invalid_id : when_replacing_a_group
    {
        Establish context = () =>
        {
            ExistingGroup = CreateGroup(new Group {DisplayName = "existing group"});

            GroupId = "bogus id";

            GroupDto = new Group
            {
                Id = "bogus id",
                DisplayName = "new group",
                ExternalId = "hello",
            };
        };

        It should_return_method_not_found = () => Response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);

        private static Group ExistingGroup;
    }
}