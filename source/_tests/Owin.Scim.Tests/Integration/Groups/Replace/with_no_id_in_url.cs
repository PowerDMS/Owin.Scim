namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    using v2.Model;

    public class with_no_id_in_url : when_replacing_a_group
    {
        Establish context = () =>
        {
            ExistingGroup = CreateGroup(new ScimGroup2 {DisplayName = "existing group"});

            GroupId = string.Empty;

            GroupDto = new ScimGroup2
            {
                Id = ExistingGroup.Id,
                DisplayName = "new group",
                ExternalId = "hello",
            };
        };

        It should_return_method_not_allowed = () => Response.StatusCode.ShouldEqual(HttpStatusCode.MethodNotAllowed);

        private static ScimGroup ExistingGroup;
    }
}