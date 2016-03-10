namespace Owin.Scim.Tests.Integration.Groups.Replace
{
    using System.Net;

    using Machine.Specifications;

    using Model.Groups;

    public class with_missing_display_name : when_replacing_a_group
    {
        Establish context = () =>
        {
            ExistingGroup = CreateGroup(new Group {DisplayName = "existing group"});

            GroupId = ExistingGroup.Id;

            GroupDto = new Group
            {
                Id = GroupId,
                ExternalId = "hello",
            };
        };

        It should_return_bad_request = () => Response.StatusCode.ShouldEqual(HttpStatusCode.BadRequest);

        It should_return_invalid_value = () => Error.ScimType.ShouldEqual(Model.ScimErrorType.InvalidValue);

        It should_return_indicate_missing_attribute = () => Error.Detail.ShouldContain("displayName");

        private static Group ExistingGroup;
    }
}